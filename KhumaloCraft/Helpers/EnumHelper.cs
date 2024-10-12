using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Helpers;

/// <summary>
/// Note that <see cref="DisplayNameAttribute"/> and anything that derives from it cannot be applied to an enum member, and is thus not considered.
/// </summary>
public static class EnumHelper
{
    private class EnumAttributeKey
    {
        public Type EnumType;
        public Type AttributeType;

        public EnumAttributeKey(Type enumType, Type attributeType)
        {
            EnumType = enumType;
            AttributeType = attributeType;
        }

        public override int GetHashCode()
        {
            return EnumType.GetHashCode() ^ AttributeType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is not EnumAttributeKey key)
            {
                return false;
            }

            return key.EnumType.Equals(EnumType)
                && key.AttributeType.Equals(AttributeType);
        }
    }

    private class EnumAttributeData
    {
        public readonly Dictionary<Enum, Attribute[]> EnumAttributes = new Dictionary<Enum, Attribute[]>();
        public object AttributeData;
    }

    private static readonly ConcurrentDictionary<EnumAttributeKey, EnumAttributeData> _enumAttributeCache = new ConcurrentDictionary<EnumAttributeKey, EnumAttributeData>();

    private static EnumAttributeData GetEnumAttributeData(Type enumType, Type attributeType)
    {
        ArgumentNullException.ThrowIfNull(enumType);

        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Enum type expected");
        }

        ArgumentNullException.ThrowIfNull(attributeType);

        var key = new EnumAttributeKey(enumType, attributeType);

        if (_enumAttributeCache.TryGetValue(key, out var enumAttributeData))
        {
            return enumAttributeData;
        }

        enumAttributeData = new EnumAttributeData();

        foreach (Enum item in Enum.GetValues(enumType))
        {
            var attributes = enumType.GetField(item.ToString()).GetCustomAttributes(attributeType, false) as Attribute[];

            enumAttributeData.EnumAttributes.Add(item, attributes);
        }

        _enumAttributeCache.TryAdd(key, enumAttributeData);

        return enumAttributeData;
    }

    public static T TryGetAttribute<T>(Enum value) where T : Attribute
    {
        var attributes = TryGetAttributes<T>(value);

        if (attributes.IsNullOrEmpty())
        {
            return default;
        }

        return attributes.FirstOrDefault();
    }

    public static IEnumerable<T> TryGetAttributes<T>(Enum value) where T : Attribute
    {
        ArgumentNullException.ThrowIfNull(value);

        var attributeData = GetEnumAttributeData(value.GetType(), typeof(T));

        if (attributeData != null && attributeData.EnumAttributes.TryGetValue(value, out Attribute[] results))
        {
            return results as T[];
        }

        return null;
    }

    public static bool TryGetAttributeValue<TAttribute, T>(Enum field, Func<TAttribute, T> valueExpression, out T value) where TAttribute : Attribute
    {
        var attribute = TryGetAttribute<TAttribute>(field);

        if (attribute == null)
        {
            value = default;

            return false;
        }

        value = valueExpression(attribute);

        return true;
    }

    public static T GetAttribute<T>(Enum value) where T : Attribute
    {
        var result = TryGetAttribute<T>(value);

        return result == null ? throw new Exception($"Attribute {typeof(T).Name} not applied to enumeration item {value}.") : result;
    }

    public static T GetAttributeValue<TAttribute, T>(Enum field, Func<TAttribute, T> valueExpression, T fallback) where TAttribute : Attribute
    {
        var attribute = TryGetAttribute<TAttribute>(field);
        if (attribute == null)
        {
            return fallback;
        }

        return valueExpression(attribute);
    }

    public static IEnumerable<T> GetAttributes<T>(Enum value) where T : Attribute
    {
        var results = TryGetAttributes<T>(value);

        return results == null ? throw new Exception($"Attribute {typeof(T).Name} not applied to enumeration item {value}.") : results;
    }

    public static IDictionary<Enum, T> GetAttributes<T>(Type enumType) where T : Attribute
    {
        return GetEnumAttributeData(enumType, typeof(T)).EnumAttributes.Where(item => item.Value != null)
            .ToDictionary(item => item.Key, item => item.Value.First() as T);
    }

    public static IDictionary<Enum, IEnumerable<T>> GetAllAttributes<T>(Type enumType) where T : Attribute
    {
        return GetEnumAttributeData(enumType, typeof(T)).EnumAttributes.Where(item => item.Value != null)
            .ToDictionary(item => item.Key, item => item.Value as IEnumerable<T>);
    }

    public static bool HasAttribute<T>(Enum value) where T : Attribute
    {
        var attribute = TryGetAttribute<T>(value);

        return attribute != null;
    }

    public static bool ShouldIgnore(Enum item)
    {
        if (HasAttribute<IgnoreEnumAttribute>(item))
        {
            return true;
        }

        if (TryGetAttributeValue<BrowsableAttribute, bool>(item, a => a.Browsable, out var isBrowsable) && !isBrowsable)
        {
            return true;
        }

        return false;
    }

    private static string GetDisplayedName(Enum value)
    {
        return GetDisplayedName(value, value.ToString());
    }

    private static string GetDisplayedName(Enum value, string fallback)
    {
        return GetAttributeValue<DisplayAttribute, string>(value, a => a.Name, fallback);
    }

    /// <summary>
    /// Looks for the following attributes in the following order and returns the first one that is present and has a value that is not null or only whitespace:
    /// <see cref="DescriptionAttribute"/>, <see cref="DisplayAttribute"/>.
    /// If none of these attributes exist, returns the provided fallback string; if that is unspecified or explicitly null, the ToString() name of the enum value.
    /// </summary>
    /// <param name="field"></param>
    /// <param name="fallback"></param>
    /// <returns></returns>
    public static string GetBestDescription(this Enum field, string fallback = null)
    {
        if (field == null)
        {
            return null;
        }

        if (TryGetAttributeValue<DescriptionAttribute, string>(field, a => a.Description, out string description))
        {
            return description;
        }

        if (TryGetAttributeValue<DisplayAttribute, string>(field, a => a.Name, out description))
        {
            return description;
        }

        return fallback ?? field.ToString();
    }

    public static string GetDescriptionOrDisplayedName(Enum value)
    {
        var description = GetDescriptionOrNull(value);

        return string.IsNullOrWhiteSpace(description) ? GetDisplayedName(value) : description;
    }

    public static string GetDescriptionOrDisplayedNameOrSpaceOnUpper(Enum value)
    {
        if (value == null) return null;

        var description = GetDescriptionOrNull(value);

        if (!string.IsNullOrWhiteSpace(description))
            return description;

        return GetDisplayedName(value, value.ToString().SpaceOnUpper());
    }

    public enum ComparisonType
    {
        CaseSensitive,
        CaseInsensitive,
    }

    public static IDictionary<string, int> ConvertToDictionary(Type enumeration)
    {
        if (!enumeration.IsEnum)
        {
            throw new ArgumentException("Enum type expected.");
        }

        var names = Enum.GetNames(enumeration);
        var values = Enum.GetValues(enumeration);

        var items = new Dictionary<string, int>();

        for (var i = 0; i < names.Length; i++)
        {
            items.Add(names[i], Convert.ToInt32(values.GetValue(i)));
        }

        return items;
    }

    public static bool TryGetEnumFromDescription<T>(string text, ComparisonType comparisonType, out T value)
    {
        object @enum = TryGetEnumFromDescription(typeof(T), text, comparisonType);
        if (@enum != null)
        {
            value = (T)@enum;

            return true;
        }

        value = default;

        return false;
    }

    private class DescriptionSpecificData
    {
        public Dictionary<string, Enum> CaseInsensitiveDescriptionToEnum;
        public Dictionary<string, Enum> CaseSensitiveDescriptionToEnum;
    }

    public static Enum TryGetEnumFromDescription(Type enumType, string value, ComparisonType comparisonType)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var attributeData = GetEnumAttributeData(enumType, typeof(DescriptionAttribute));
        var descriptionSpecificData = attributeData.AttributeData as DescriptionSpecificData;
        if (descriptionSpecificData == null)
        {
            var dsd = new DescriptionSpecificData
            {
                CaseInsensitiveDescriptionToEnum = new Dictionary<string, Enum>(),
                CaseSensitiveDescriptionToEnum = new Dictionary<string, Enum>(),
            };

            foreach (var item in attributeData.EnumAttributes)
            {
                if (!(item.Value is DescriptionAttribute[] descriptionAttributes)
                    || descriptionAttributes.IsNullOrEmpty())
                {
                    continue;
                }

                var description = descriptionAttributes.First().Description;
                dsd.CaseInsensitiveDescriptionToEnum[(description != null) ? description.ToLower() : description] = item.Key;
                dsd.CaseSensitiveDescriptionToEnum[description] = item.Key;
            }

            descriptionSpecificData = dsd;
            attributeData.AttributeData = dsd;
        }

        Enum result;
        switch (comparisonType)
        {
            case ComparisonType.CaseSensitive:
                if (!descriptionSpecificData.CaseSensitiveDescriptionToEnum.TryGetValue(value.ToLower(), out result))
                {
                    result = null;
                }
                break;

            case ComparisonType.CaseInsensitive:
                if (!descriptionSpecificData.CaseInsensitiveDescriptionToEnum.TryGetValue(value.ToLower(), out result))
                {
                    result = null;
                }
                break;

            default:
                throw new Exception("Unknown comparison type.");
        }

        return result;
    }

    public static Enum GetEnumFromDescription(Type enumType, string value, ComparisonType comparisonType)
    {
        var e = TryGetEnumFromDescription(enumType, value, comparisonType);
        if (e == null)
        {
            throw new Exception($"Enum of type \"{enumType.Name}\" for description \"{value}\" not found.");
        }

        return e;
    }

    public static T EnumFromDescription<T>(string value)
        where T : struct, Enum
    {
        return EnumFromDescription<T>(value, ComparisonType.CaseInsensitive);
    }

    public static T EnumFromDescription<T>(string value, ComparisonType comparisonType)
        where T : struct, Enum
    {
        return (T)GetEnumFromDescription(typeof(T), value, comparisonType);
    }

    public static string GetDescription(Type enumType, int? value)
    {
        if (value == null)
        {
            return null;
        }

        return GetDescriptionOrDisplayedName((Enum)FromInt(enumType, value));
    }

    public static string GetDescription(Enum value)
    {
        return GetDescription(value, value.ToString());
    }

    private static string GetDescription(Enum value, string fallback)
    {
        if (TryGetAttributeValue<DescriptionAttribute, string>(value, a => a.Description, out var description))
        {
            return description;
        }

        return string.IsNullOrEmpty(fallback) ? null : fallback;
    }

    public static string GetDescriptionOrNull(Enum value)
    {
        return value != null ? GetDescription(value, null) : null;
    }

    public static List<string> GetDescriptions<T>()
        where T : struct, Enum
    {
        return GetDescriptions(typeof(T));
    }

    public static List<string> GetDescriptions<T>(Func<Enum, string> nameExtractor = null)
        where T : struct, Enum
    {
        return GetDescriptions(typeof(T), nameExtractor);
    }

    public static List<string> GetDescriptions(Type type)
    {
        return GetDescriptions(type, GetDescription);
    }

    public static List<string> GetDescriptions(Type type, Func<Enum, string> nameExtractor = null)
    {
        if (!type.IsEnum)
        {
            throw new ArgumentException("Enum type expected.");
        }

        // default: preserve case
        if (nameExtractor == null)
        {
            nameExtractor = name => name.ToString();
        }

        var descriptions = new List<string>();

        foreach (Enum item in Enum.GetValues(type))
        {
            descriptions.Add(nameExtractor(item));
        }

        return descriptions;
    }

    public static readonly Func<Enum, string> DefaultNameExtractor = GetDescriptionOrDisplayedName;

    public static IEnumerable<KeyValuePair<string, int?>> Enumerate<TEnum>(bool spaceOnUpper = false, bool keepEnumOrder = false)
      where TEnum : struct, Enum
    {
        Comparison<KeyValuePair<string, int?>> valueSorter = null;
        if (keepEnumOrder)
        {
            valueSorter = (l, r) => Nullable.Compare(l.Value, r.Value);
        }

        return Enumerate(typeof(TEnum), item => spaceOnUpper
            ? GetDescriptionOrDisplayedNameOrSpaceOnUpper(item)
            : DefaultNameExtractor(item), e => !ShouldIgnore(e), sorter: valueSorter);
    }

    public static IEnumerable<KeyValuePair<string, int?>> Enumerate<T>(
            Func<Enum, string> nameExtractor,
            Func<Enum, bool> shouldInclude = null,
            Comparison<KeyValuePair<string, int?>> sorter = null,
            bool prependEmptyItemIfNullable = true)
        where T : struct, Enum
    {
        return Enumerate(typeof(T), nameExtractor, shouldInclude, sorter, prependEmptyItemIfNullable);
    }

    public static IEnumerable<KeyValuePair<string, int?>> Enumerate(
            Type enumType,
            Func<Enum, string> nameExtractor = null,
            Func<Enum, bool> shouldInclude = null,
            Comparison<KeyValuePair<string, int?>> sorter = null,
            bool prependEmptyItemIfNullable = true)
    {
        if (enumType == null)
        {
            throw new ArgumentNullException(nameof(enumType));
        }

        enumType = enumType.GetEnumActualType(out bool isNullableEnum);
        if (!enumType.IsEnum)
        {
            throw new ArgumentException($"Specified type {enumType} is not an enum");
        }

        // preserve case
        if (nameExtractor == null)
        {
            nameExtractor = DefaultNameExtractor;
        }

        // include everything
        if (shouldInclude == null)
        {
            shouldInclude = e => true;
        }

        // sort alphabetically
        if (sorter == null)
        {
            sorter = (l, r) => l.Key.CompareTo(r.Key);
        }

        var names = GetDescriptions(enumType, nameExtractor);
        var values = Enum.GetValues(enumType);

        var items = new List<KeyValuePair<string, int?>>();

        for (var i = 0; i < names.Count; i++)
        {
            var e = (Enum)values.GetValue(i);

            if (!shouldInclude(e))
            {
                continue;
            }

            var value = (int)Convert.ChangeType(e, typeof(int));
            items.Add(new KeyValuePair<string, int?>(names[i], value));
        }

        items.Sort(sorter);

        if (isNullableEnum && prependEmptyItemIfNullable)
        {
            items.Insert(0, new KeyValuePair<string, int?>(null, null));
        }

        return items;
    }

    public static IEnumerable<KeyValuePair<string, int?>> EnumerateBrowsable<T>()
    {
        return Enumerate(typeof(T), shouldInclude: e =>
        {
            var browsableAttribute = TryGetAttribute<BrowsableAttribute>(e);

            return browsableAttribute?.Browsable != false;
        });
    }

    public static TEnum? TryParse<TEnum>(string value)
        where TEnum : struct, Enum
    {
        return TryParse<TEnum>(value, ignoreCase: true);
    }

    public static TEnum? TryParse<TEnum>(string value, bool ignoreCase)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse(value, ignoreCase, out TEnum result))
        {
            return result;
        }

        return default;
    }

    public static T Parse<T>(string value)
        where T : struct, Enum
    {
        return Parse<T>(value, true);
    }

    public static T Parse<T>(string value, bool ignoreCase)
        where T : struct, Enum
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static object FromInt(Type enumType, int? value)
    {
        if (value == null)
        {
            return null;
        }

        return Enum.ToObject(enumType.GetEnumActualType(), value.Value);
    }

    public static TEnum? FromInt<TEnum>(int? value)
        where TEnum : struct, Enum
    {
        return (TEnum?)Enum.ToObject(typeof(TEnum), value);
    }

    public static TEnum? TryFromIntDefined<TEnum>(int? value)
        where TEnum : struct, Enum
    {
        if (!value.HasValue || !Enum.IsDefined(typeof(TEnum), value.Value))
        {
            return null;
        }

        return (TEnum)Enum.ToObject(typeof(TEnum), value.Value);
    }

    /// <summary>
    /// Returns the actual type of the specified enum type argument.
    /// </summary>
    /// <param name="this">The type to check.</param>
    /// <param name="isNullable">If the supplied type is an enum, specifies if that type is nullable; else false.</param>
    /// <returns>The actual type of the specified enum type argument, or null if the type is not an enum type.</returns>
    public static Type GetEnumActualType(this Type @this, out bool isNullable)
    {
        if (@this == null)
        {
            isNullable = false;

            return null;
        }

        isNullable = @this.IsGenericType && (@this.GetGenericTypeDefinition() == typeof(Nullable<>));

        if (isNullable)
        {
            @this = @this.GetGenericArguments()[0];
        }

        if (@this.IsEnum)
        {
            return @this;
        }

        return null;
    }

    public static Type GetEnumActualType(this Type @this)
    {
        return GetEnumActualType(@this, out bool isNullable);
    }

    public static IEnumerable<T> GetValues<T>(Type enumType)
        where T : struct, Enum
    {
        return (T[])Enum.GetValues(enumType);
    }

    public static IEnumerable<T> GetValues<T>()
        where T : struct, Enum
    {
        return GetValues<T>(typeof(T));
    }

    public static IEnumerable<T> GetValuesExcluding<T>(T[] enumsToExclude)
        where T : struct, Enum
    {
        return GetValues<T>().Except(enumsToExclude);
    }

    public static IEnumerable<T> GetValues<T>(Func<T, bool> predicate)
        where T : struct, Enum
    {
        foreach (var value in Enum.GetValues(typeof(T)))
        {
            var enumValue = (T)value;

            if (predicate(enumValue))
            {
                yield return enumValue;
            }
        }
    }

    public static List<T> AsList<T>(Type enumType)
        where T : struct, Enum
    {
        return GetValues<T>(enumType).ToList();
    }

    public static List<T> AsList<T>()
        where T : struct, Enum
    {
        return GetValues<T>().ToList();
    }

    public static int Count<T>()
        where T : struct, Enum
    {
        return GetValues<T>().Count();
    }

    /// <summary>
    /// Returns a dictionary that uses each enum's description as its value, or the tostring value of the enum if no description is present.
    /// </summary>
    public static Dictionary<T, string> AsDictionary<T>(IEqualityComparer<T> keyComparer = null)
        where T : struct, Enum
    {
        return AsDictionary<T, DescriptionAttribute, string>(a => a.Description, e => e.ToString(), keyComparer);
    }

    public static Dictionary<T, TAttributeValue> AsDictionary<T, TAttribute, TAttributeValue>(Func<TAttribute, TAttributeValue> valueExpression, Func<T, TAttributeValue> fallback = null, IEqualityComparer<T> keyComparer = null)
        where T : struct, Enum
        where TAttribute : Attribute
    {
        if (fallback == null)
        {
            fallback = _ => default;
        }

        return Enum.GetValues(typeof(T)).Cast<T>()
            .ToDictionary(k => k, v => GetAttributeValue<TAttribute, TAttributeValue>(v, valueExpression, fallback(v)), keyComparer);
    }

    public static IEnumerable<TEnum> OrderBy<TEnum>(Func<TEnum, int> keySelector)
        where TEnum : struct, Enum
    {
        return GetValues<TEnum>().OrderBy(keySelector);
    }

    // http://stackoverflow.com/a/22222260/70345
    public static IEnumerable<Enum> GetUniqueFlags(this Enum flags)
    {
        var flag = 1ul;
        foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
        {
            var bits = Convert.ToUInt64(value);
            while (flag < bits)
            {
                flag <<= 1;
            }

            if (flag == bits && flags.HasFlag(value))
            {
                yield return value;
            }
        }
    }

    // the split between value/values is to prevent someone from calling this method with no params
    public static bool In<T>(this T @this, T value, params T[] values)
        where T : struct, Enum
    {
        var comparer = EqualityComparer<T>.Default;

        if (comparer.Equals(@this, value))
        {
            return true;
        }

        return values.Any(v => comparer.Equals(@this, v));
    }
}
