namespace KhumaloCraft.Imaging;

public static class GeneralExtentions
{
    public static bool Contains<T>(this T[] array, T item)
    {
        foreach (T i in array)
            if (i.Equals(item))
                return true;
        return false;
    }

    public static bool Contains<T>(this Enum value, T item)
        where T : struct, Enum
    {
        foreach (T i in Enum.GetValues(typeof(T)))
            if (i.Equals(item))
                return true;
        return false;
    }

    public static bool TryGetAttribute<V>(this Enum @enum, out V attribute)
        where V : Attribute
    {
        attribute = default;
        var type = @enum.GetType();
        var members = type.GetMember(@enum.ToString());
        if (0 == members.Length)
            return false;
        var attributes = members[0].GetCustomAttributes(typeof(V), false);
        if (0 == attributes.Length)
            return false;
        attribute = (V)attributes[0];
        return true;
    }

    public static V GetAttribute<V>(this Enum value)
        where V : Attribute
    {
        if (!value.TryGetAttribute<V>(out V result))
            throw new NotImplementedException("Attribute " + typeof(V).Name + " not applied to enumeration item.");
        return result;
    }
}