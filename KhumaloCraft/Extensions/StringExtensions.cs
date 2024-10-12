using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace KhumaloCraft;

public static class StringExtensions
{
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static string Left(this string value, int count)
    {
        if (value == null)
            return null;

        return value.Substring(0, Math.Min(count, value.Length));
    }

    public static string Right(this string value, int count)
    {
        if (value == null)
            return null;

        var startPos = Math.Max(count, value.Length) - count;

        return value.Substring(startPos);
    }

    public static string RemoveEscapeCharactersAndTrim(this string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            //use string.split and string.join
            var escapeCharacters = new[] { '\n', '\t', '\v', '\b', '\r', '\f' };
            var tokens = value.Trim().Split(escapeCharacters);

            if (tokens.Length > 1)
            {
                return string.Concat(tokens);
            }
        }

        return value;
    }

    public static string RemoveDoubleSpacesAndTrim(this string content)
    {
        while (content.Contains("  "))
        {
            content = content.Replace("  ", " ");
        }
        return content.Trim();
    }

    public static string RemoveCRLF(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        char? nullChar = null;

        var result = new StringBuilder();
        for (var iChar = 0; iChar < value.Length; iChar++)
        {
            var current = value[iChar];
            var next = iChar < value.Length - 1 ? value[iChar + 1] : nullChar;

            if (current.IsCRLF())
            {
                if (next.HasValue && !next.Value.IsCRLF())
                {
                    result.Append(' ');
                }
                continue;
            }
            result.Append(current);
        }
        return result.ToString();
    }

    public static bool IsCRLF(this char value)
    {
        return value == '\n' || value == '\r';
    }

    public static string CropToLength(this string value, int length)
    {
        return (value == null || value.Length <= length) ? value : value.Substring(0, length);
    }

    public static string CropToLength(this string value, int length, string suffix)
    {
        return (value == null || value.Length <= length) ? value : string.Concat(value.Substring(0, length), suffix);
    }

    public static string Reverse(this string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length == 1)
            return value;

        var chars = value.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public static bool Contains(this string inputValue, string comparisonValue, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        return (inputValue.IndexOf(comparisonValue, comparisonType) != -1);
    }

    /// <summary>Convert text's case to a title case</summary>
    public static string ToTitleCase(this string value, CultureInfo culture)
    {
        return culture.TextInfo.ToTitleCase(value);
    }

    public static string[] Split(this string value, string regexPattern, RegexOptions options)
    {
        return Regex.Split(value, regexPattern, options);
    }

    public static string[] GetWords(this string value)
    {
        return value.Split(' ');
    }

    public static string GetWordByIndex(this string value, int index)
    {
        var words = value.GetWords();

        if ((index < 0) || (index > words.Length - 1))
            throw new IndexOutOfRangeException("The word number is out of range.");

        return words[index];
    }

    public static string RemoveAllSpecialCharacters(this string value)
    {
        var sb = new StringBuilder(value.Length);
        foreach (var c in value.Where(c => char.IsLetterOrDigit(c)))
            sb.Append(c);
        return sb.ToString();
    }

    // aka ToCamelCase
    public static string SpaceOnUpper(this string value)
    {
        return Regex.Replace(value, @"(\p{Lu})(?=\p{Ll})|(?<=\p{Ll})(\p{Lu}|\d+)", " $1$2", RegexOptions.Compiled).TrimStart();
    }

    public static bool ContainsAny(this string @this, params string[] values)
    {
        return @this.ContainsAny(StringComparison.CurrentCulture, values);
    }

    /// <summary>
    /// Determines whether the string contains any of the provided values.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="comparisonType"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAny(this string @this, StringComparison comparisonType, params string[] values)
    {
        foreach (var value in values)
        {
            if (@this.IndexOf(value, comparisonType) > -1)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether the string contains all of the provided values.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool ContainsAll(this string @this, params string[] values)
    {
        return @this.ContainsAll(StringComparison.CurrentCulture, values);
    }

    public static bool ContainsAll(this string @this, StringComparison comparisonType, params string[] values)
    {
        foreach (var value in values)
        {
            if (@this.IndexOf(value, comparisonType) == -1)
                return false;
        }

        return true;
    }

    public static string ReplaceAll(this string @this, char[] search, char replace)
    {
        var result = @this;

        foreach (var c in search)
        {
            result = result.Replace(c, replace);
        }

        return result;
    }

    public static string StripAll(this string @this, char[] search)
    {
        var result = @this;

        foreach (var c in search)
        {
            result = result.Replace(c.ToString(), string.Empty);
        }

        return result;
    }

    public static bool EqualsAny(this string @this, StringComparison comparisonType, params string[] values)
    {
        foreach (var value in values)
        {
            if (@this.Equals(value, comparisonType))
                return true;
        }

        return false;
    }

    public static bool IsLike(this string @this, string pattern)
    {
        if (@this == pattern)
            return true;
        if (string.IsNullOrEmpty(@this))
            return false;

        if (pattern[0] == '*' && pattern.Length > 1)
        {
            for (var index = 0; index < @this.Length; index++)
            {
                if (@this.Substring(index).IsLike(pattern.Substring(1)))
                    return true;
            }
        }

        if (pattern[0] == '*')
        {
            return true;
        }

        if (pattern[0] == @this[0])
        {
            return @this.Substring(1).IsLike(pattern.Substring(1));
        }

        return false;
    }

    public static bool IsEquivalentTo(this string value, string compareWith)
    {
        if (value == null || compareWith == null)
            return false;

        return string.Compare(value, compareWith, true) == 0;
    }

    public static string Truncate(this string @this, int length, bool useEllipses = false)
    {
        var e = useEllipses ? 3 : 0;
        if (length - e <= 0)
            throw new InvalidOperationException($"Length must be greater than {e}.");

        if (string.IsNullOrEmpty(@this) || @this.Length <= length)
            return @this;

        return @this.Substring(0, length - e) + new string('.', e);
    }

    public static string TrimAndAddTrailingPeriod(this string item)
    {
        if (item == null)
            return item;

        return item.Trim().TrimEnd('.') + ".";
    }

    public static string TrimEnd(this string value, string endsWith, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (value != null
            && endsWith != null
            && value.Length >= endsWith.Length
            && value.EndsWith(endsWith, comparisonType))
        {
            return value.Substring(0, value.Length - endsWith.Length);
        }

        return value;
    }

    public static IEnumerable<string> Split(this string str, Func<char, bool> controller)
    {
        var nextPiece = 0;

        for (var c = 0; c < str.Length; c++)
        {
            if (controller(str[c]))
            {
                yield return str.Substring(nextPiece, c - nextPiece);
                nextPiece = c + 1;
            }
        }

        yield return str.Substring(nextPiece);
    }

    /// <summary>
    /// Tests whether the contents of a string is a numeric value
    /// </summary>
    /// <param name="value">String to check</param>
    /// <returns>Boolean indicating whether or not the string contents are numeric</returns>
    public static bool IsNumeric(this string value)
    {
        if (value.IsNullOrEmpty())
            return false;

        return double.TryParse(value.Trim(), out _);
    }

    /// <summary>
    /// Try and parse to int
    /// </summary>
    /// <param name = "value">String to check</param>
    /// <returns>An integer value parsed from string, Null if parsing failed</returns>
    public static int? TryParseToInt(this string value)
    {
        if (string.IsNullOrEmpty(value)) return null;

        if (int.TryParse(value, out var output))
        {
            return output;
        }

        return null;
    }

    /// <summary>
    /// Try and parse to int
    /// </summary>
    /// <param name = "value">String to check</param>
    /// <returns>An integer value parsed from string, Null if parsing failed</returns>
    public static long? TryParseToLong(this string value)
    {
        if (long.TryParse(value, out var output))
        {
            return output;
        }

        return null;
    }

    /// <summary>
    /// Attempts to convert the specific comma-separated string to an array of the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <param name="throwOnConversionFailure"></param>
    /// <returns></returns>
    public static IEnumerable<T> ToArray<T>(this string str, bool throwOnConversionFailure = false)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        var converter = TypeDescriptor.GetConverter(typeof(T));

        if (!converter.CanConvertFrom(str.GetType()))
        {
            throw new ArgumentException($"Unable to convert from {str.GetType()}");
        }

        if (!converter.CanConvertTo(typeof(T)))
        {
            throw new ArgumentException($"Unable to convert to {typeof(T)}");
        }

        var strArray = str.Split(',');
        var results = new List<T>(strArray.Length);

        foreach (var element in strArray)
        {
            try
            {
                results.Add((T)converter.ConvertFromString(element));
            }
            catch
            {
                if (throwOnConversionFailure)
                {
                    throw;
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Try and parse to int
    /// </summary>
    /// <param name="value">String to check</param>
    /// <returns>An integer value parsed from string, Null if parsing failed</returns>
    public static int[] TryParseToIntArray(this string[] value)
    {
        if (value == null)
            return null;
        if (value.Length == 0)
            return new int[0];

        var result = new List<int>();
        foreach (var val in value)
        {
            var @int = val.TryParseToInt();

            if (!@int.HasValue)
                continue;

            result.Add(@int.Value);
        }

        return result.ToArray();
    }

    public static decimal? TryParseToDecimal(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return null;

        if (decimal.TryParse(value, out var output))
        {
            return output;
        }

        return null;
    }

    public static DateTime? TryParseToDateTime(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (DateTime.TryParse(value, out var output))
        {
            return output;
        }

        return null;
    }

    // http://stackoverflow.com/a/21327150/70345
    public static IEnumerable<string> SplitCamelCase(this string @this)
    {
        return Regex.Split(@this, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})", RegexOptions.Compiled);
    }

    public static string ValueOrEmpty(this string instance)
    {
        return string.IsNullOrEmpty(instance) ? string.Empty : instance.Trim();
    }

    /// <summary>
    /// Allows substringing with a negative startIndex. In that case, the substring position is interpreted as an offset from the end of the specified string.
    /// </summary>
    /// <param name="this"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static string SubstringEx(this string @this, int startIndex)
    {
        if (startIndex < 0)
        {
            return @this.Substring(0, @this.Length + startIndex);
        }

        return @this.Substring(startIndex);
    }

    public static bool In(this string text, IEnumerable<string> items, IEqualityComparer<string> comparer)
    {
        return items.Contains(text, comparer);
    }

    public static bool In(this string text, IEnumerable<string> items, StringComparison comparisonType = default)
    {
        return In(text, items, comparisonType.ToComparer());
    }

    public static StringComparer ToComparer(this StringComparison @this)
    {
        switch (@this)
        {
            case StringComparison.CurrentCulture: return StringComparer.CurrentCulture;

            case StringComparison.CurrentCultureIgnoreCase: return StringComparer.CurrentCultureIgnoreCase;

            case StringComparison.InvariantCulture: return StringComparer.InvariantCulture;

            case StringComparison.InvariantCultureIgnoreCase: return StringComparer.InvariantCultureIgnoreCase;

            case StringComparison.Ordinal: return StringComparer.Ordinal;

            case StringComparison.OrdinalIgnoreCase: return StringComparer.OrdinalIgnoreCase;

            default: throw new InvalidEnumArgumentException(nameof(@this), (int)@this, typeof(StringComparison));
        }
    }

    public static bool ContainsLetter(this string text)
    {
        return text.Any(char.IsLetter);
    }

    public static bool ContainsNumber(this string text)
    {
        return text.Any(char.IsNumber);
    }

    public static bool ContainsNonNumeric(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        return value.Any(ch => !char.IsDigit(ch));
    }

    public static bool ContainsWhitespace(this string text)
    {
        return text.Any(ch => char.IsWhiteSpace(ch));
    }

    public static string Replace(this string original, string pattern, string replacement, StringComparison comparisonType)
    {
        return Replace(original, pattern, replacement, comparisonType, -1);
    }

    public static string Replace(this string original, string pattern, string replacement, StringComparison comparisonType, int stringBuilderInitialSize)
    {
        if (original == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(pattern))
        {
            return original;
        }

        var posCurrent = 0;
        var lenPattern = pattern.Length;
        var idxNext = original.IndexOf(pattern, comparisonType);
        var result = new StringBuilder(stringBuilderInitialSize < 0 ? Math.Min(4096, original.Length) : stringBuilderInitialSize);

        while (idxNext >= 0)
        {
            result.Append(original, posCurrent, idxNext - posCurrent);
            result.Append(replacement);

            posCurrent = idxNext + lenPattern;

            idxNext = original.IndexOf(pattern, posCurrent, comparisonType);
        }

        result.Append(original, posCurrent, original.Length - posCurrent);

        return result.ToString();
    }

    public static string RemoveWhiteSpace(this string input)
    {
        var cleanedCharArray = input?.Trim().ToLowerInvariant()
            .Where(i => !char.IsWhiteSpace(i))
            .ToArray();
        return new string(cleanedCharArray);
    }

    public static string RemoveWhiteSpacePuncAndSymbol(this string input)
    {
        var cleanedCharArray = input?.Trim().ToLowerInvariant()
            .Where(i => (!char.IsWhiteSpace(i) && !char.IsPunctuation(i) && !char.IsSymbol(i)))
            .ToArray();
        return new string(cleanedCharArray);
    }

    /// <summary>
    /// If the supplied string contains the specified character, returns that string until that character; otherwise the original string.
    /// </summary>
    /// <param name="this">The string to search and substring.</param>
    /// <param name="value">The character value to search for.</param>
    /// <returns>A substring or the original string.</returns>
    public static string SubstringToIfPresent(this string @this, char value)
    {
        var index = @this.IndexOf(value);
        if (index != -1)
        {
            return @this.Substring(0, index);
        }

        return @this;
    }

    public static string SubstringToIfPresent(this string @this, string value)
    {
        var index = @this.IndexOf(value);
        if (index != -1)
        {
            return @this.Substring(0, index);
        }

        return @this;
    }

    public static string FirstLetterToUpperCase(this string s)
    {
        if (string.IsNullOrEmpty(s))
            return string.Empty;
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    public static bool StartsWithVowel(this string value)
    {
        if (value == null)
        {
            return false;
        }

        return "aeiou".Contains(value[0].ToString(), StringComparison.InvariantCultureIgnoreCase);
    }

    public static string GetIndefiniteArticle(this string value)
    {
        return value.StartsWithVowel() ? "an" : "a";
    }

    public static string TrimPunctuation(this string value)
    {
        int firstNonPunctuationIndex = 0;

        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsPunctuation(value[i]))
            {
                firstNonPunctuationIndex = i;
                break;
            }
        }

        int lastNonPunctuationIndex = value.Length - 1;

        for (int i = value.Length - 1; i >= 0; i--)
        {
            if (!char.IsPunctuation(value[i]))
            {
                lastNonPunctuationIndex = i;
                break;
            }
        }

        if (firstNonPunctuationIndex != 0 || lastNonPunctuationIndex != value.Length - 1)
        {
            return value.Substring(firstNonPunctuationIndex, lastNonPunctuationIndex - firstNonPunctuationIndex + 1);
        }
        return value;
    }
}
