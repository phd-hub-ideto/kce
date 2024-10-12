using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Hosting;

public static class SameSiteCookieConfig
{
    public static void Use(IApplicationBuilder app)
    {
        app.UseCookiePolicy(new CookiePolicyOptions()
        {
            MinimumSameSitePolicy = SameSiteMode.Lax,
            Secure = CookieSecurePolicy.Always,
            OnAppendCookie = OnAppendCookie,
            OnDeleteCookie = OnDeleteCookie
        });
    }

    private static void OnAppendCookie(AppendCookieContext cookieContext)
    {
        if (cookieContext.CookieName == Cookie.AuthenticationCookie.GetBestDescription())
        {
            cookieContext.CookieOptions.SameSite = SameSiteMode.None;
        }

        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
    }

    private static void OnDeleteCookie(DeleteCookieContext cookieContext)
    {
        if (cookieContext.CookieName == Cookie.AuthenticationCookie.GetBestDescription())
        {
            cookieContext.CookieOptions.SameSite = SameSiteMode.None;
        }

        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
    }

    private static void CheckSameSite(HttpContext context, CookieOptions options)
    {
        if (options.SameSite == SameSiteMode.None)
        {
            var userAgent = context.Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.UserAgent].ToString();

            if (SameSiteCookieHelper.DisallowsSameSiteNone(userAgent))
            {
                options.SameSite = SameSiteMode.Unspecified;
            }
        }
    }
}