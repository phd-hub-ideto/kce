using KhumaloCraft.Dependencies;
using SimpleInjector;

namespace KhumaloCraft;

public static class KhumaloCraftDependencies
{
    public static void Register(Container container, EntryPoint entryPoint)
    {
        container.RegisterUsingAttributes(typeof(KhumaloCraftDependencies).Assembly);
    }
}