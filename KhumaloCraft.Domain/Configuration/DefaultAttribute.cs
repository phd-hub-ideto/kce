namespace KhumaloCraft.Domain;

internal class DefaultAttribute : Attribute
{
    public DefaultAttribute(object value)
    {
        Value = value;
    }

    public object Value { get; }
}