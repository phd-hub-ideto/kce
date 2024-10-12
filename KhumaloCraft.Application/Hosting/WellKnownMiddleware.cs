using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;

namespace KhumaloCraft.Application.Hosting;

internal class WellKnownMiddleware : IMiddleware
{
    private readonly string _location;

    public WellKnownMiddleware(IOptions<WellKnownMiddlewareOptions> options)
    {
        _location = options.Value.ChangePasswordUri.ToString();
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.Equals("/.well-known/change-password", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = (int)HttpStatusCode.SeeOther;

            context.Response.Headers.Location = _location;
            return;
        }

        await next(context);
    }
}