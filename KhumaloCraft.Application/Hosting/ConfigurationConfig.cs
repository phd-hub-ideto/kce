using KhumaloCraft.Configuration.Providers;
using KhumaloCraft.Domain.Configuration;
using Microsoft.Extensions.Configuration;

namespace KhumaloCraft.Application.Hosting;

public static class ConfigurationConfig
{
    public static void Configure(IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Add(new DatabaseConfigurationSource());

        var interimConfiguration = configurationBuilder.Build();

        //USE interimConfiguration when you need interim settings for further configuration or checks
        //bool enableAzureKeyVault = Convert.ToBoolean(interimConfiguration["AzureKeyVaultEnabled"]);

        configurationBuilder.AddJsonFilesRecursive();

        configurationBuilder.AddEnvironmentVariables();
    }
}
