using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace KhumaloCraft.Application.Hosting;

public static class SecurityHeadersConfig
{
    private static readonly TimeSpan _oneYear = TimeSpan.FromDays(365);

    public static void Add(IServiceCollection service)
    {
        service.AddHsts(options =>
        {
            options.MaxAge = _oneYear;
            options.IncludeSubDomains = true;
        });
    }

    public static void Use(IApplicationBuilder app)
    {
        app.UseHsts();

        app.Use(async (ctx, next) =>
        {
            var response = ctx.Response;

            response.OnStarting(() =>
            {
                response.Headers.TryAdd("X-Frame-Options", "SAMEORIGIN");
                response.Headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
                response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
                return Task.CompletedTask;
            });

            await next(ctx);
        });
    }
}