using System.Collections.Concurrent;
using System.Reflection;

namespace KhumaloCraft.Reflection;

public static class AttributeCache
{
    private static class TypeInfoCache<T>
    {
        public static ConcurrentDictionary<Tuple<Type, bool>, IEnumerable<T>> Cache = new ConcurrentDictionary<Tuple<Type, bool>, IEnumerable<T>>();
    }

    public static IEnumerable<T> GetCustomAttributes<T>(Type type, bool inherit = false)
        where T : Attribute
    {
        return TypeInfoCache<T>.Cache.GetOrAdd(Tuple.Create(type, inherit), key => type.GetCustomAttributes<T>(inherit));
    }

    private static class MethodInfoCache<T>
    {
        public static readonly ConcurrentDictionary<Tuple<MethodInfo, bool>, IEnumerable<T>> Cache = new ConcurrentDictionary<Tuple<MethodInfo, bool>, IEnumerable<T>>();
    }

    public static IEnumerable<T> GetCustomAttributes<T>(MethodInfo methodInfo, bool inherit = false)
        where T : Attribute
    {
        return MethodInfoCache<T>.Cache.GetOrAdd(Tuple.Create(methodInfo, inherit), key => methodInfo.GetCustomAttributes<T>(inherit));
    }

    private static class PropertyInfoCache<T>
    {
        public static readonly ConcurrentDictionary<Tuple<PropertyInfo, bool>, IEnumerable<T>> Cache = new ConcurrentDictionary<Tuple<PropertyInfo, bool>, IEnumerable<T>>();
    }

    public static IEnumerable<T> GetCustomAttributes<T>(PropertyInfo propertyInfo, bool inherit = false)
        where T : Attribute
    {
        return PropertyInfoCache<T>.Cache.GetOrAdd(Tuple.Create(propertyInfo, inherit), key => propertyInfo.GetCustomAttributes<T>(inherit));
    }

    /*
    private static class FieldInfoCache<T>
    {
        public static readonly ConcurrentDictionary<Tuple<FieldInfo, bool>, IEnumerable<T>> Cache = new ConcurrentDictionary<Tuple<FieldInfo, bool>, IEnumerable<T>>();
    }

    public static IEnumerable<T> GetCustomAttributes<T>(FieldInfo field, bool inherit = false)
        where T : Attribute
    {
        return FieldInfoCache<T>.Cache.GetOrAdd(Tuple.Create(field, inherit), key => field.GetCustomAttributes<T>(inherit));
    }
    */

    private static class FieldInfoCache
    {
        public static readonly ConcurrentDictionary<Tuple<FieldInfo, Type, bool>, IEnumerable<Attribute>> Cache = new ConcurrentDictionary<Tuple<FieldInfo, Type, bool>, IEnumerable<Attribute>>();
    }

    public static IEnumerable<T> GetCustomAttributes<T>(FieldInfo field, bool inherit = false)
        where T : Attribute
    {
        var attributeType = typeof(T);

        return (IEnumerable<T>)FieldInfoCache.Cache.GetOrAdd(Tuple.Create(field, attributeType, inherit), key => field.GetCustomAttributes(attributeType, inherit).Cast<Attribute>());
    }

    public static IEnumerable<Attribute> GetCustomAttributes(FieldInfo field, Type attributeType, bool inherit = false)
    {
        return FieldInfoCache.Cache.GetOrAdd(Tuple.Create(field, attributeType, inherit), key => field.GetCustomAttributes(attributeType, inherit).Cast<Attribute>());
    }
}
