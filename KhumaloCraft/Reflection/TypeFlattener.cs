using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace KhumaloCraft.Reflection;

// https://josef.codes/transform-csharp-objects-to-a-flat-string-dictionary/
public static class TypeFlattener
{
    private static readonly ConcurrentDictionary<Type, Dictionary<PropertyInfo, Func<object, object>>> _cachedProperties = new();

    public static Dictionary<string, string> Flatten(object @object, string prefix = "")
    {
        return FlattenInternal(@object, prefix: prefix);
    }

    private static Dictionary<string, string> FlattenInternal(object @object, Dictionary<string, string> dictionary = default, string prefix = "")
    {
        dictionary ??= new Dictionary<string, string>();
        var type = @object.GetType();
        var properties = GetProperties(type);

        foreach (KeyValuePair<PropertyInfo, Func<object, object>> p in properties)
        {
            var property = p.Key;
            var getter = p.Value;

            var key = string.IsNullOrWhiteSpace(prefix) ? property.Name : $"{prefix}.{property.Name}";
            var value = getter(@object);

            if (value == null)
            {
                dictionary[key] = null;
                continue;
            }

            if (IsValueTypeOrString(property.PropertyType))
            {
                dictionary[key] = ToStringValueType(value);
            }
            else if (property.PropertyType.FullName == "System.Net.IPAddress") // we need to deal with IPAddress specially :( 
            {
                dictionary[key] = ToStringValueType(value);
            }
            else
            {
                if (value is IEnumerable enumerable)
                {
                    var counter = 0;
                    foreach (var item in enumerable)
                    {
                        var itemKey = $"{key}[{counter++}]";
                        var itemType = item.GetType();
                        if (IsValueTypeOrString(itemType))
                        {
                            dictionary[itemKey] = ToStringValueType(item);
                        }
                        else
                        {
                            FlattenInternal(item, dictionary, itemKey);
                        }
                    }
                }
                else
                {
                    FlattenInternal(value, dictionary, key);
                }
            }
        }

        return dictionary;
    }

    private static Dictionary<PropertyInfo, Func<object, object>> GetProperties(Type type)
    {
        if (_cachedProperties.TryGetValue(type, out var properties))
        {
            return properties;
        }

        CacheProperties(type);
        return _cachedProperties[type];
    }

    private static void CacheProperties(Type type)
    {
        if (_cachedProperties.ContainsKey(type))
        {
            return;
        }

        _cachedProperties[type] = new Dictionary<PropertyInfo, Func<object, object>>();
        var properties = type.GetProperties().Where(p => p.CanRead && p.GetIndexParameters().Length == 0);
        foreach (var propertyInfo in properties)
        {
            var getter = CompilePropertyGetter(propertyInfo);
            _cachedProperties[type].Add(propertyInfo, getter);
            if (!IsValueTypeOrString(propertyInfo.PropertyType))
            {
                if (IsIEnumerable(propertyInfo.PropertyType))
                {
                    var types = propertyInfo.PropertyType.GetGenericArguments();
                    foreach (var genericType in types)
                    {
                        if (!IsValueTypeOrString(genericType))
                        {
                            CacheProperties(genericType);
                        }
                    }
                }
                else
                {
                    CacheProperties(propertyInfo.PropertyType);
                }
            }
        }
    }

    // Inspired by Zanid Haytam
    // https://blog.zhaytam.com/2020/11/17/expression-trees-property-getter/
    private static Func<object, object> CompilePropertyGetter(PropertyInfo property)
    {
        var objectType = typeof(object);
        var objectParameter = Expression.Parameter(objectType);
        var castExpression = Expression.TypeAs(objectParameter, property.DeclaringType);
        var convertExpression = Expression.Convert(Expression.Property(castExpression, property), objectType);

        return Expression.Lambda<Func<object, object>>(convertExpression, objectParameter).Compile();
    }

    private static bool IsIEnumerable(Type type) => type.GetInterfaces().Contains(typeof(IEnumerable));

    private static bool IsValueTypeOrString(Type type) => type.IsValueType || type == typeof(string);

    private static string ToStringValueType(object value)
    {
        return value switch
        {
            DateTime dateTime => dateTime.ToString("o"),
            bool boolean => boolean.ToString(),
            _ => value.ToString()
        };
    }

}