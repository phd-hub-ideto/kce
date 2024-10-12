using KhumaloCraft.Configuration.Providers;
using Microsoft.Extensions.Configuration;

namespace KhumaloCraft.Configuration;

public static class StaticConfiguration
{
    private static IConfigurationRoot _configurationRoot;

    static StaticConfiguration()
    {
        var builder = new ConfigurationBuilder();

        builder.AddJsonFilesRecursive();

        // Add environment variables (including Azure App Service settings)
        builder.AddEnvironmentVariables();

        _configurationRoot = builder.Build();
    }

    public static string ConnectionString(DatabaseConnectionName name)
    {
        return name switch
        {
            DatabaseConnectionName.KhumaloCraft => _configurationRoot.GetConnectionString(ConnectionStringName.KhumaloCraftDatabase),
            _ => throw new ArgumentException("Unexpected DatabaseConnectionName value"),
        };
    }

    public static IConfiguration Configuration => _configurationRoot;

}