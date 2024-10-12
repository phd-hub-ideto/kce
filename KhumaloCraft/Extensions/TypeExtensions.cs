using System.Reflection;
using System.Runtime.CompilerServices;

namespace KhumaloCraft;

public static class TypeExtensions
{
    private static readonly Type _objectType = typeof(object);

    /// <summary>
    /// Gets the first supertype in the type hierarchy of the current type that has a single generic parameter that is
    /// either of the given type or is a subclass of the given type.
    /// Returns null if no such supertype is found.
    /// </summary>
    /// <typeparam name="A">The type of the supertype's single generic argument.</typeparam>
    /// <param name="type"></param>
    public static Type GetGenericSupertype<A>(this Type type)
    {
        var argumentType = typeof(A);

        var supertype = type.BaseType;
        while (supertype?.Equals(_objectType) == false)
        {
            if (supertype.IsGenericType || supertype.IsGenericTypeDefinition)
            {
                var arguments = supertype.IsGenericTypeDefinition ? supertype.GetGenericArguments() : supertype.GenericTypeArguments;

                var argument = arguments.Length == 1 ? arguments[0] : null;
                if (argument != null && (argument.Equals(argumentType) || argument.IsSubclassOf(argumentType)))
                {
                    return supertype;
                }
            }
            supertype = supertype.BaseType;
        }
        return null;
    }

    // https://stackoverflow.com/a/2483054/70345
    public static bool IsAnonymous(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        // HACK: The only way to detect anonymous types right now.
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
            && type.IsGenericType && type.Name.Contains("AnonymousType")
            && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
            && type.Attributes.HasFlag(TypeAttributes.NotPublic);
    }

    public static bool IsAssignableToGenericType(this Type type, Type genericType)
    {
        foreach (var it in type.GetInterfaces())
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
            return true;

        var baseType = type.BaseType;
        if (baseType == null)
            return false;

        return IsAssignableToGenericType(baseType, genericType);
    }
}