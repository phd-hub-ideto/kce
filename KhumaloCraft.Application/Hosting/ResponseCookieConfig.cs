using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Helpers;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace KhumaloCraft.Application.Hosting;

public static class ResponseCookieConfig
{
    private class MediaTypeComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return obj.ToLowerInvariant().GetHashCode();
        }
    }

#pragma warning disable ATTJS, ATEJS
    private static readonly HashSet<string> _allowedMediaTypesForCookies = new HashSet<string>(new MediaTypeComparer())
    {
        ContentType.Html,
        ContentType.Json,
        ContentType.Javascript, //this is obsolete so should be removed at some point.
        ContentType.ExperimentalJavascript,
        ContentType.ApplicationJavascript
    };
#pragma warning restore ATTJS, ATEJS

    public static void Use(IApplicationBuilder app)
    {
        app.Use(async (ctx, next) =>
        {
            var request = ctx.Request;
            var response = ctx.Response;

            response.OnStarting(() =>
            {
                var headers = response.GetTypedHeaders();

                if (headers.SetCookie.Count > 0)
                {
                    var httpContextProvider = app.ApplicationServices.GetService<IHttpContextProvider>();

                    var allowCookiesInResponse = httpContextProvider.AllowCookiesInResponse;

                    httpContextProvider.AllowCookiesInResponse = false;

                    var clearCookies = response.StatusCode == StatusCodes.Status304NotModified;

                    if (!clearCookies)
                    {
                        if (response.StatusCode == StatusCodes.Status200OK)
                        {
                            clearCookies = !_allowedMediaTypesForCookies.Contains(headers.ContentType?.MediaType.Value);
                        }
                        else
                        {
                            clearCookies = !allowCookiesInResponse;
                        }
                    }

                    if (clearCookies)
                    {
                        response.Headers.Remove(Microsoft.Net.Http.Headers.HeaderNames.SetCookie);
                    }
                }

                var formsAuthenticationCookie = Cookie.FormsAuthentication.GetBestDescription();

                if (request.Cookies.ContainsKey(formsAuthenticationCookie))
                {
                    var httpCookies = app.ApplicationServices.GetRequiredService<IHttpCookies>();

                    httpCookies.RemoveCookie(formsAuthenticationCookie);
                }

                return Task.CompletedTask;
            });

            await next(ctx);
        });
    }
}