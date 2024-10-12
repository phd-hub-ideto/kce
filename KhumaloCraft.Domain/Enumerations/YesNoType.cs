namespace KhumaloCraft.Domain.Enumerations;

public enum YesNoType
{
    Yes = 1,
    No = 2
}

public static class YesNoTypeHelper
{
    public static bool? ToBoolean(YesNoType? value)
    {
        if (value == null) return null;

        return (value == YesNoType.Yes);
    }

    public static YesNoType? FromBoolean(bool? value)
    {
        if (value == null) return null;
        return (value == true ? YesNoType.Yes : YesNoType.No);
    }
}