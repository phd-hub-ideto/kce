using SimpleInjector;
using System.Reflection;

namespace KhumaloCraft.Dependencies;

public static class ContainerExtensions
{
    public static Container RegisterDerivedConcreteTypes(this Container container, Type openGenericType, Assembly assembly, Lifestyle lifestyle)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsInterface && !type.IsAbstract && (type.IsAssignableToGenericType(openGenericType) || type.IsAssignableTo(openGenericType)))
            {
                container.Register(type, type, lifestyle);
            }
        }

        return container;
    }

    public static Container RegisterSingletonCollectionFromAssembly(this Container container, Type registrationType, Assembly assembly)
    {
        RegisterCollectionFromAssembly(container, registrationType, assembly, Lifestyle.Singleton);

        return container;
    }

    public static Container RegisterCollectionFromAssembly(this Container container, Type registrationType, Assembly assembly, Lifestyle lifestyle)
    {
        var types = container.GetTypesToRegister(registrationType, [assembly,]);

        var registrations = types
            .Select(type => lifestyle.CreateRegistration(type, container))
            .ToArray();

        foreach (var registration in registrations)
        {
            container.Collection.Append(registrationType, registration);
        }

        return container;
    }
}
