using KhumaloCraft.Application.Hosting;

namespace KhumaloCraft.Application.ImageServer;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(ConfigurationConfig.Configure)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging((hostContext, logging) =>
            {
                if (hostContext.HostingEnvironment.IsProduction())
                {
                    logging.ClearProviders();
                }
            });
    }
}
