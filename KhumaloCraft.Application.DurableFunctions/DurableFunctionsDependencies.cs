using KhumaloCraft.Dependencies;
using SimpleInjector;

namespace KhumaloCraft.Application.DurableFunctions;

public static class DurableFunctionsDependencies
{
    public static void Register(Container container)
    {
        var currentAssembly = typeof(DurableFunctionsDependencies).Assembly;

        container.RegisterUsingAttributes(currentAssembly);
    }
}