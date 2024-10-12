using KhumaloCraft.Application.Session;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

namespace KhumaloCraft.Application.Hosting;

public static class UniqueUserIdMiddlewareConfig
{
    public static void Use(IApplicationBuilder builder)
    {
        builder.UseMiddleware<UniqueUserIdMiddleware>();
    }

    public static void Add(IServiceCollection services, Container container)
    {
        services.AddSingleton(_ => container.GetInstance<IUniqueUserTracker>());
        services.AddSingleton<UniqueUserIdMiddleware>();
    }
}