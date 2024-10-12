using KhumaloCraft.Application.Dependencies;
using KhumaloCraft.Application.Hosting;
using KhumaloCraft.Application.Monitoring;
using KhumaloCraft.Application.Platforms;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Reflection;

namespace KhumaloCraft.Application.ImageServer;

public class Startup
{
    private Container _container;

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        _container = DependencyManager.ConfigureDefaultContainer(_ => { }, false);

        services.AddControllersWithViews()
            .ConfigureApplicationPartManager(setup =>
            {
                // Prevents accidental registration of controllers not in this assembly.
                var applicationParts = setup.ApplicationParts;

                foreach (var applicationPart in setup.ApplicationParts.ToList())
                {
                    if (applicationPart is AssemblyPart assemblyPart
                        && assemblyPart.Assembly != Assembly.GetExecutingAssembly()
                        && assemblyPart.Assembly.GetName().Name.StartsWith("KhumaloCraft"))
                    {
                        applicationParts.Remove(assemblyPart);
                    }
                }
            });

        services.AddHttpClient();

        ConfigureSimpleInjectorContainer(_container, services);

        SecurityHeadersConfig.Add(services);
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
            //https://github.com/simpleinjector/SimpleInjector/issues/748
            //.AddTagHelperActivation();
        });

        GlobalDependencies.Register(container, EntryPoint.ImageServer, Configuration);

        container.RegisterSingleton<IRandomSeedProvider, NewGuidRandomSeedProvider>();
        container.RegisterSingleton<IPlatformDeterminator, UnknownPlatformDeterminator>();
        container.RegisterSingleton<ITemplateNameProvider, Application.ImageServer.ImageServerTemplateNameProvider>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        SecurityHeadersConfig.Use(app);

        app.UseSimpleInjector(_container);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        DenyHttpHeadRequestsConfig.Use(app);

        StaticFilesConfig.Use(app, env, _container, EntryPoint.ImageServer);

        app.UseMiddleware<RequestCountersMiddleware>(_container);
        app.UseRouting();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        _container.Verify();
    }
}