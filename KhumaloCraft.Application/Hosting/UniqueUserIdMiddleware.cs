using KhumaloCraft.Application.Session;
using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Hosting;

internal class UniqueUserIdMiddleware : IMiddleware
{
    private readonly IUniqueUserTracker _uniqueUserTracker;

    public UniqueUserIdMiddleware(IUniqueUserTracker uniqueUserTracker)
    {
        _uniqueUserTracker = uniqueUserTracker;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _uniqueUserTracker.EnsureCookie();

        await next(context);
    }
}