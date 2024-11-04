using KhumaloCraft.Application.Authentication.Basic;
using KhumaloCraft.Application.Browsers;
using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Application.Dependencies;
using KhumaloCraft.Application.DisplayModes;
using KhumaloCraft.Application.Hosting;
using KhumaloCraft.Application.Monitoring;
using KhumaloCraft.Application.Portal.Hubs;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Authentication.Passwords;
using KhumaloCraft.Domain.Dates;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Helpers;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Text.Json.Serialization;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace KhumaloCraft.Application.Portal;

public class Startup
{
    private Container _container;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;

        //DO NOT REMOVE
        //var throwaway = SqlGeography.Null;
        //SqlServerTypes.Utilities.LoadNativeAssemblies(environment.ContentRootPath);
        //END DO NOT REMOVE
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        _container = DependencyManager.ConfigureDefaultContainer(_ => { }, false);

        services.AddSignalR();
        //TODO-LP : Enabled Durable Functions Orchestrator
        /*
        services.AddDurableClientFactory(options =>
        {
            options.TaskHub = "OrderProcessingHub";
        });

        services.AddSingleton<IDurableOrchestrationClient>(provider =>
        {
            var context = provider.GetRequiredService<IDurableClientFactory>();
            return context.CreateClient();
        });*/

        var mvcBuilder = services.AddControllersWithViews()
        .AddJsonOptions(j =>
        {
            j.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        })
        .AddMvcOptions(options =>
        {
            ModelValidationConfig.Add(options, _container);
            ContentSecurityPolicyMiddlewareConfig.AddMvcOptions(options);
        });

        if (Environment.IsDevelopment())
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }

        ConcurrencyLimiterConfig.Add(services, queueLimit: 5000);

        CompressionConfig.Add(services);

        //TODO-L : Maybe Configure RouteOptions ConstraintResolver.Configure(routeOptions.ConstraintMap);
        /*
        ''services.Configure<RouteOptions>(routeOptions =>
        {
            ConstraintResolver.Configure(routeOptions.ConstraintMap);
        });*/

        CookieAuthenticationConfig.Add(services, _container, "/login", "/logout");

        services.Configure<RazorViewEngineOptions>(o =>
        {
            o.ViewLocationExpanders.Add(new CustomViewLocationExpander(_container.GetInstance<IDisplayModeSelector>()));
            o.ViewLocationFormats.Add("/{2}/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            o.ViewLocationFormats.Add("/{2}/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            o.ViewLocationFormats.Add("/Common/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            o.ViewLocationFormats.Add("/Common/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
        });

        services.AddSession(options =>
        {
            options.Cookie.Name = Cookie.PortalSession.GetBestDescription();
        });

        services.AddSingleton<BasicAuthenticationHandler>();
        services.AddSingleton(_ => _container.GetInstance<IPasswordValidator>());
        services.AddSingleton(_ => _container.GetInstance<IBasicAuthenticationHelper>());
        services.AddSingleton(_ => _container.GetInstance<IDisplayModeSelector>());
        services.AddSingleton(_ => _container.GetInstance<IBrowserDetector>());
        services.AddSingleton(_ => _container.GetInstance<ISettings>());
        services.AddSingleton(_ => _container.GetInstance<IHttpContextProvider>());
        services.AddSingleton(_ => _container.GetInstance<IDateProvider>());
        services.AddSingleton(_ => _container.GetInstance<IHttpCookies>());
        services.AddSingleton(_ => _container.GetInstance<IUserLocationCookiePersistor>());
        services.AddSingleton(_ => _container.GetInstance<IPrincipalResolver>());
        services.AddSingleton(_ => _container.GetInstance<IUserRolePermissionRepository>());
        services.AddSingleton(_ => _container.GetInstance<IUserRoleRepository>());

        services.AddHttpClient();

        services.AddHttpForwarder();

        UniqueUserIdMiddlewareConfig.Add(services, _container);

        NoContentResultFilterConfig.Add(services);

        ConfigureSimpleInjectorContainer(_container, services);

        SecurityHeadersConfig.Add(services);

        services.AddWellKnown(options =>
        {
            var baseUrl = new Uri(_container.GetInstance<ISettings>().PortalBaseUri);

            options.ChangePasswordUri = new Uri(baseUrl, Constants.Pages.PasswordChangeUrl);
        });
    }

    private void ConfigureSimpleInjectorContainer(Container container, IServiceCollection services)
    {
        container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

        services.AddSimpleInjector(container, options =>
        {
            options
                .AddAspNetCore()
                .AddControllerActivation()
                .AddViewComponentActivation()
                .AddPageModelActivation();
        });

        GlobalDependencies.Register(container, EntryPoint.Portal, Configuration);
        PortalDependencies.Register(container);

        ModelValidationConfig.Register(container);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpForwarder forwarder, ITransformBuilder transformBuilder)
    {
        app.UseSimpleInjector(_container);

        // *Important*, a lot can go wrong when cookies get cached, this must remain the first middleware to get executed.
        CookieCacheability.Use(app);

        // Use authentication first to set up the user context
        app.UseAuthentication();

        SiteVisitLogMiddlewareConfig.Use(app, _container);

        ReturnUrlValidationMiddlewareConfig.Use(app, _container);

        SecurityHeadersConfig.Use(app);

        ResponseBufferingConfig.Use(app, context => context.Request.Path.StartsWithSegments("/Bundles"));

        ConcurrencyLimiterConfig.Use(app);

        ConfigureErrorHandler(env, app);

        ResponseCookieConfig.Use(app);

        DenyHttpHeadRequestsConfig.Use(app);

        CompressionConfig.Use(app);

        StaticFilesConfig.Use(app, env, _container, EntryPoint.Portal, env.ContentRootPath);

        RedirectToRemoveTrailingSlashesConfig.Use(app);

        UniqueUserIdMiddlewareConfig.Use(app);

        SameSiteCookieConfig.Use(app);

        app.UseMiddleware<RequestCountersMiddleware>(_container);

        app.UseRouting();

        app.UseSession();

        //Moved to the top just before SiteVisitLogMiddlewareConfig middleware to set up the user context before logging
        //app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<NotificationHub>("/notificationHub");

            var httpTransform = transformBuilder.Create(context =>
            {
                context.AddPathRemovePrefix("/answers");

                context.AddRequestHeaderRemove("Cookie");

                var headers = new HashSet<string>(
                [
                    EnumHelper.GetDescription(Cookie.FormsAuthentication),
                    EnumHelper.GetDescription(Cookie.UserId)
                ], StringComparer.InvariantCultureIgnoreCase);

                context.AddRequestTransform(requestTransformContext =>
                {
                    var request = requestTransformContext.HttpContext.Request;

                    var typedHeaders = request.GetTypedHeaders();
                    typedHeaders.Cookie = typedHeaders.Cookie.RemoveAll(item => headers.Contains(item.Name.ToString())).ToList();

                    string cookieValue = request.Headers.Cookie.ToString();

                    requestTransformContext.ProxyRequest.Headers.Add("Cookie", cookieValue);

                    return ValueTask.CompletedTask;
                });
            });

            //TODO-L: Implement answerbase
            //endpoints.MapForwarder("/answers/{**catch-all}", "https://khumalocraft.answerbase.com", ForwarderRequestConfig.Empty, httpTransform);

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        app.UseWellKnown();

        _container.Verify();
    }

    private static void ConfigureErrorHandler(IWebHostEnvironment env, IApplicationBuilder app)
    {
        //TODO : Revert and remove !
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(new ExceptionHandlerOptions()
            {
                ExceptionHandlingPath = "/errors/500"
            });

            app.UseStatusCodePagesWithReExecute("/errors/{0}");
        }
    }
}