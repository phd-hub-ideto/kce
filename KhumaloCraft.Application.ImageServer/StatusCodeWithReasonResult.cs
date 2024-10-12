using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace KhumaloCraft.Application.ImageServer;

public class StatusCodeWithReasonResult : IActionResult
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _reasonPhrase;

    public StatusCodeWithReasonResult(HttpStatusCode statusCode, string reasonPhrase)
    {
        _statusCode = statusCode;
        _reasonPhrase = reasonPhrase;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;

        response.StatusCode = (int)_statusCode;
        response.HttpContext
                .Features
                .Get<IHttpResponseFeature>()
                .ReasonPhrase = _reasonPhrase;
    }

}