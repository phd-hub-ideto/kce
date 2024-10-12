using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace KhumaloCraft.Reflection;

public static class TypeHelpers
{
    private static readonly ConcurrentDictionary<Type, Type> _itemTypes = new ConcurrentDictionary<Type, Type>();

    public static Type GetItemType(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return _itemTypes.GetOrAdd(type, t =>
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments()[0];
            }

            return t;
        });
    }

    public static bool IsAnonymousType<T>(T instanceOfObject)
        where T : class
    {
        if (instanceOfObject == null)
            return false;

        return IsAnonymousType(instanceOfObject.GetType());
    }

    private static readonly ConcurrentDictionary<Type, bool> _isAnonymousTypeCache = new ConcurrentDictionary<Type, bool>();

    public static bool IsAnonymousType(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return _isAnonymousTypeCache.GetOrAdd(type, t =>
        {
            return Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute), false)
                && t.IsGenericType
                && (t.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic
                && t.Name.Contains("AnonymousType")
                && (t.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) || t.Name.Contains(".<>"));
        });
    }
}