using Microsoft.AspNetCore.Builder;
using System.Net;

namespace KhumaloCraft.Application.Hosting;

public class DenyHttpHeadRequestsConfig
{
    public static void Use(IApplicationBuilder app)
    {
        app.Use(async (ctx, next) =>
        {
            if (ctx.Request.Method == "HEAD")
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return;
            }

            await next(ctx);
        });
    }
}
