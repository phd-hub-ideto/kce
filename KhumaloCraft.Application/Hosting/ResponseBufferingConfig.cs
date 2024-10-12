using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Hosting;

public static class ResponseBufferingConfig
{
    public static void Use(IApplicationBuilder applicationBuilder, Func<HttpContext, bool> predicate)
    {
        applicationBuilder.Use(async (context, next) =>
        {
            if (predicate(context))
            {
                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    long length = 0;

                    context.Response.OnStarting(() =>
                    {
                        context.Response.Headers.ContentLength = length;
                        return Task.CompletedTask;
                    });

                    await next(context);

                    length = context.Response.Body.Length;

                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream, context.RequestAborted);
                }
            }
            else
            {
                await next(context);
            }
        });
    }
}