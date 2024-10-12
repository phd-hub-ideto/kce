using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace KhumaloCraft.Application.Hosting;

public static class CookieCacheability
{
    /// <summary>
    /// This ensures that the response caching middleware and external caches never get hold of something with a set-cookie header.
    /// </summary>
    /// <param name="app"></param>
    public static void Use(IApplicationBuilder app)
    {
        app.Use(async (ctx, next) =>
        {
            var response = ctx.Response;

            response.OnStarting(() =>
            {
                var headers = response.Headers;

                if (headers.SetCookie != StringValues.Empty
                    && headers.CacheControl.ToString() != "private")
                {
                    headers.CacheControl = "private";

                    headers.Remove(HeaderNames.Expires);
                    headers.Remove(HeaderNames.LastModified);
                    headers.Remove(HeaderNames.ETag);
                    headers.Remove(HeaderNames.Age);
                }

                return Task.CompletedTask;
            });

            await next(ctx);
        });
    }
}