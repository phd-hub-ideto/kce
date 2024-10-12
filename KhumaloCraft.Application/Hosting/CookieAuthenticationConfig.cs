using KhumaloCraft.Application.Authentication;
using KhumaloCraft.Application.Authentication.Basic;
using KhumaloCraft.Application.DataProtection;
using KhumaloCraft.Application.Http;
using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Helpers;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using SimpleInjector;

namespace KhumaloCraft.Application.Hosting;

public static class CookieAuthenticationConfig
{
    public static void Add(IServiceCollection services, Container container, string loginPath, string logoutPath)
    {
        KhumaloCraftDataProtectionProvider.Add(services);

        services
            .AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.AddScheme<BasicAuthenticationHandler>(BasicAuthenticationHandler.AuthenticationScheme, "basic");
            })
            .AddCookie(options =>
            {
                var dpProvider = container.GetInstance<IDataProtectionProvider>();

                options.TicketDataFormat = new AuthenticationCookieTicketFormat(
                    dpProvider.CreateProtector(AuthenticationCookieTicketFormat.CookieDataProtectionPurpose),
                    container.GetInstance<IHttpContextProvider>(),
                    container.GetInstance<IUserRepository>(),
                    container.GetInstance<IUserRolePermissionRepository>(),
                    container.GetInstance<IUserRoleRepository>()
                );
                options.Cookie.Name = Cookie.AuthenticationCookie.GetBestDescription();
                options.Cookie.Domain = container.GetInstance<IHttpCookies>().Domain;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.LoginPath = loginPath;
                options.LogoutPath = logoutPath;
                options.ExpireTimeSpan = TimeSpan.FromDays(90);
                options.SlidingExpiration = false;
                options.Events.OnValidatePrincipal = ctx =>
                {
                    var principal = ctx.Principal as KhumaloCraftPrincipal;

                    if (principal != null && principal.User.Deleted)
                    {
                        ctx.RejectPrincipal();

                        var formsAuth = container.GetRequiredService<IFormsAuthenticator>();

                        formsAuth.Signout();
                    }

                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToLogin = ctx =>
                {
                    //The return URL is typically the path of the current AJAX request, but we can't redirect, post login, to said path.
                    //So rather try and redirect, post login, to the page that the AJAX request was made from.
                    if (ctx.HttpContext.Request.IsAjaxRequest())
                    {
                        var currentRedirectUri = new UriBuilder(ctx.RedirectUri);
                        var queryString = QueryStringBuilder.Create(currentRedirectUri.Query);
                        var referrer = ctx.HttpContext.Request.Headers.Referer;

                        if (queryString.Get("ReturnUrl") is not null && referrer != StringValues.Empty)
                        {
                            queryString.Set("ReturnUrl", new Uri(referrer).AbsolutePath);

                            currentRedirectUri.Query = queryString.Build();

                            ctx.Response.Headers.Location = currentRedirectUri.ToString();
                        }
                        else
                        {
                            ctx.Response.Headers.Location = ctx.RedirectUri;
                        }
                        ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        ctx.Response.Redirect(ctx.RedirectUri);
                    }
                    return Task.CompletedTask;
                };
            });
    }
}