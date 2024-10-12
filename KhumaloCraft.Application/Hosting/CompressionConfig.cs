using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;

namespace KhumaloCraft.Application.Hosting;

public static class CompressionConfig
{
    public static void Add(IServiceCollection services)
    {
        services.AddResponseCompression(configureOptions =>
        {
            configureOptions.EnableForHttps = true;
            configureOptions.MimeTypes = ResponseCompressionDefaults.MimeTypes;
            configureOptions.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });
    }

    public static void Use(IApplicationBuilder app)
    {
        app.UseResponseCompression();
    }
}