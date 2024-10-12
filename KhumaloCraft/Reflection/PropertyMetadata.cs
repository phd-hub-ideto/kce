using System.Reflection;

namespace KhumaloCraft.Reflection;

public class PropertyMetadata
{
    public PropertyMetadata(string name, Type type, Func<object, object> getter, bool canWrite)
    {
        Name = name;
        Type = type;
        Getter = getter;
        CanWrite = canWrite;
    }

    public string Name { get; set; }

    public Type Type { get; set; }

    public Func<object, object> Getter { get; set; }

    public bool CanWrite { get; set; }

    public override string ToString()
    {
        return Name;
    }

    private static Dictionary<Type, PropertyMetadata[]> _cache = new Dictionary<Type, PropertyMetadata[]>();

    public static IEnumerable<PropertyMetadata> Get(Type type)
    {
        PropertyMetadata[] result;
        lock (_cache)
        {
            if (_cache.TryGetValue(type, out result))
            {
                return result;
            }
        }

        var propertyMetadata = new List<PropertyMetadata>();
        var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in propertyInfos)
        {
            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                continue;
            }
            var getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
            {
                continue;
            }
            var getter = DynamicProperty.CreateGetter(propertyInfo);
            propertyMetadata.Add(new PropertyMetadata(propertyInfo.Name, propertyInfo.PropertyType, target =>
            {
                object obj;
                try
                {
                    obj = getter(target);
                }
                catch (Exception e) when (e is MemberAccessException || e is TypeAccessException)
                {
                    getter = t => propertyInfo.GetValue(t, null);
                    obj = getter(target);
                }

                return obj;
            },
            propertyInfo.CanWrite));
        }

        result = propertyMetadata.ToArray();

        lock (_cache)
        {
            _cache[type] = result;
        }

        return result;
    }
}
