using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace KhumaloCraft.Application.Hosting;

public static class ConcurrencyLimiterConfig
{
    private static readonly int _concurrencyLimit = Environment.ProcessorCount * 25;
    private static readonly int _queueLimit = _concurrencyLimit * 2;

    static ConcurrencyLimiterConfig()
    {
        ThreadPool.SetMinThreads(_concurrencyLimit * 2, Environment.ProcessorCount);
    }

    public static void Add(IServiceCollection services, int? concurrencyLimit = null, int? queueLimit = null)
    {
        services.AddQueuePolicy(options =>
        {
            options.MaxConcurrentRequests = concurrencyLimit ?? _concurrencyLimit;
            options.RequestQueueLimit = queueLimit ?? _queueLimit;
        });
    }

    public static void Use(IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.UseConcurrencyLimiter();
    }
}