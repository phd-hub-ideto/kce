using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace KhumaloCraft.Application.Hosting;

public static class WellKnownMiddlewareConfig
{
    /// <summary>
    /// Adds well-known services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> for adding services.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="WellKnownMiddlewareOptions"/>.</param>
    /// <returns></returns>
    public static IServiceCollection AddWellKnown(this IServiceCollection services, Action<WellKnownMiddlewareOptions> configureOptions)
    {
        services.AddSingleton<WellKnownMiddleware>();

        services.Configure(configureOptions);

        return services;
    }

    /// <summary>
    /// Adds middleware for using well-known services.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
    public static void UseWellKnown(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<WellKnownMiddleware>();
    }
}