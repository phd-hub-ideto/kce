namespace KhumaloCraft.Reflection;

public class PropertyValue
{
    private Func<object> _value;

    public PropertyValue(string name, Type type, Func<object> value)
    {
        Name = name;
        Type = type;
        _value = value;
    }

    public string Name { get; set; }

    public Type Type { get; set; }

    public object Value
    {
        get
        {
            return _value();
        }
    }

    public override string ToString()
    {
        return Name;
    }

    private static readonly PropertyValue[] _empty = { };

    public static IEnumerable<PropertyValue> Get(object values)
    {
        if (values == null)
        {
            return _empty;
        }

        var metaData = PropertyMetadata.Get(values.GetType());

        if (TypeHelpers.IsAnonymousType(values) == false)
        {
            metaData = metaData.Where(p => p.CanWrite);
        }

        return metaData.Select(p => new PropertyValue(p.Name, p.Type, () => p.Getter(values)));
    }
}