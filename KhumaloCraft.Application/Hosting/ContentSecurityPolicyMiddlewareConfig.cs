using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace KhumaloCraft.Application.Hosting;

public static class ContentSecurityPolicyMiddlewareConfig
{
    public static void Use<TMiddleware>(IApplicationBuilder builder)
        where TMiddleware : ContentSecurityPolicyMiddleware
    {
        builder.UseMiddleware<TMiddleware>();
    }

    public static void Add<TMiddleware>(IServiceCollection services)
        where TMiddleware : ContentSecurityPolicyMiddleware
    {
        services.AddSingleton<TMiddleware>();
    }

    public static void AddMvcOptions(MvcOptions options)
    {
        options.InputFormatters.OfType<SystemTextJsonInputFormatter>().First().SupportedMediaTypes.Add(
            new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(ContentType.CspReport)
        );
    }
}