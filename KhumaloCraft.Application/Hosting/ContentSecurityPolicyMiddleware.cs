using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace KhumaloCraft.Application.Hosting;

public abstract class ContentSecurityPolicyMiddleware : IMiddleware
{
    protected abstract string ContentSecurityPolicyScriptSrc { get; }
    protected abstract string ContentSecurityPolicyStyleSrc { get; }
    protected abstract string ContentSecurityPolicyConnectSrc { get; }
    protected abstract string ContentSecurityPolicyImageSrc { get; }
    protected abstract string ContentSecurityPolicyFrameSrc { get; }
    protected abstract string ContentSecurityPolicyFontSrc { get; }
    protected abstract string ContentSecurityPolicyObjectSrc { get; }
    protected abstract Uri ContentSecurityPolicyReportUri { get; }
    protected abstract bool ContentSecurityPolicyEnabled { get; }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var response = context.Response;

        response.OnStarting(() =>
        {
            if (ContentSecurityPolicyEnabled && response.Headers.ContentType.Any(ct => ct.StartsWith(ContentType.Html, StringComparison.OrdinalIgnoreCase)))
            {
                var cspHeaderValue = $"default-src 'self'; script-src {ContentSecurityPolicyScriptSrc}; style-src {ContentSecurityPolicyStyleSrc}; connect-src {ContentSecurityPolicyConnectSrc}; img-src {ContentSecurityPolicyImageSrc}; frame-src {ContentSecurityPolicyFrameSrc}; font-src {ContentSecurityPolicyFontSrc}; object-src {ContentSecurityPolicyObjectSrc}; report-uri {ContentSecurityPolicyReportUri}";

                response.Headers.TryAdd(HeaderNames.ContentSecurityPolicyReportOnly, cspHeaderValue);
            }
            return Task.CompletedTask;
        });

        await next(context);
    }
}