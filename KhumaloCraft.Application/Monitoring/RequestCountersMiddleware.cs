using Microsoft.AspNetCore.Http;

namespace KhumaloCraft.Application.Monitoring;

public class RequestCountersMiddleware : IMiddleware
{
    private readonly RequestCountersLogger _requestCountersLogger;

    public RequestCountersMiddleware(RequestCountersLogger requestCountersLogger)
    {
        _requestCountersLogger = requestCountersLogger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _requestCountersLogger.Start();

        context.Response.OnStarting(_ =>
        {
            _requestCountersLogger.Log();

            return Task.CompletedTask;

        }, null);

        await next(context);
    }
}
