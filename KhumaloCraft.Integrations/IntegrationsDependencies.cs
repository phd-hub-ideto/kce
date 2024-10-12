using KhumaloCraft.Dependencies;
using SimpleInjector;

namespace KhumaloCraft.Integrations;

public static class IntegrationsDependencies
{
    public static void Register(Container container, EntryPoint entryPoint)
    {
        container.RegisterUsingAttributes(typeof(IntegrationsDependencies).Assembly);
    }
}