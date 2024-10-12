using SimpleInjector;
using System.Reflection;

namespace KhumaloCraft.Dependencies;

public static class DeclarativeRegistrationHelper
{
    public static void RegisterUsingAttributes(this Container container, Assembly assembly)
    {
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            var registrationAttributes = type.GetCustomAttributes<RegistrationAttribute>();
            foreach (var registrationAttribute in registrationAttributes)
            {
                var lifestyleType = registrationAttribute.Lifestyle;

                Type contractType = registrationAttribute.Contract ?? GetContractType(type);

                var lifestyle = lifestyleType switch
                {
                    LifestyleType.Singleton => Lifestyle.Singleton,
                    LifestyleType.Scoped => Lifestyle.Scoped,
                    LifestyleType.Transient => Lifestyle.Transient,
                    _ => throw new NotImplementedException(),
                };

                container.Register(contractType, type, lifestyle);
            }
        }
    }

    private static Type GetContractType(Type type)
    {
        Type contractType = type;

        var interfaces = GetDirectInterfaces(type);
        if (interfaces.Count > 1)
        {
            throw new InvalidOperationException($"An interface must be specified as {type} implements multiple interfaces.");
        }
        else if (interfaces.Count == 1)
        {
            contractType = interfaces[0];
        }

        return contractType;
    }

    private static List<Type> GetDirectInterfaces(Type type)
    {
        var allInterfaces = new List<Type>();
        var childInterfaces = new List<Type>();

        foreach (var @interface in type.GetInterfaces())
        {
            allInterfaces.Add(@interface);
            foreach (var childInterface in @interface.GetInterfaces())
            {
                childInterfaces.Add(childInterface);
            }
        }

        foreach (var @interface in type.BaseType.GetInterfaces())
        {
            childInterfaces.Add(@interface);
        }

        return allInterfaces.Except(childInterfaces).ToList();

    }
}