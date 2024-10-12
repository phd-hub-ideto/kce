using Microsoft.Extensions.Configuration;

namespace KhumaloCraft.Domain.Configuration;

public class DatabaseConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return DatabaseConfigurationProvider.Instance;
    }
}