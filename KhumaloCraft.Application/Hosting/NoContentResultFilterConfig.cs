using Microsoft.Extensions.DependencyInjection;

namespace KhumaloCraft.Application.Hosting;

public static class NoContentResultFilterConfig
{
    public static void Add(IServiceCollection services)
    {
        services.AddControllers(options => options.Filters.Add<NoContentResultFilter>());
    }
}