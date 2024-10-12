using KhumaloCraft.Utilities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace KhumaloCraft.Helpers;

public static partial class FormattingHelper
{
    public static readonly string DateFormat = "dd/MM/yyyy";
    public static readonly string DateTimeFormat = $"{DateFormat} HH:mm";

    public static string FormatDateTime(DateTime value)
    {
        return value.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
    }

    public static string FormatDate(DateTime value)
    {
        return value.ToString(DateFormat, CultureInfo.InvariantCulture);
    }

    public static string FormatDateMonthName(DateTime value)
    {
        return value.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
    }

    public static string FormatTime(DateTime value) => value.ToString("HH:mm");

    public static string FormatTime(TimeSpan value) => value.ToString(@"hh\:mm");

    public static string AsFullName(string personFirstname, string personSurname)
    {
        if (personFirstname == null && personSurname == null)
            return null;

        personFirstname = string.IsNullOrEmpty(personFirstname) ? string.Empty : personFirstname.Trim();
        personSurname = string.IsNullOrEmpty(personSurname) ? string.Empty : personSurname.Trim();

        return $"{personFirstname} {personSurname}".Trim();
    }

    public static string AsSpaceSeparatedNumber(this long value)
    {
        var numberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        numberFormatInfo.NumberGroupSeparator = " ";
        return value.ToString("#,0", numberFormatInfo);
    }

    public static string AsSpaceSeparatedNumber(this decimal value)
    {
        var numberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        numberFormatInfo.NumberGroupSeparator = " ";
        return value.ToString("#,0.00", numberFormatInfo);
    }

    public const char PackUrlPartShortDash = '-';

    public static string PackUrlPart(string value)
    {
        //https://stackoverflow.com/a/2086575/
        var tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(value);
        value = Encoding.UTF8.GetString(tempBytes);

        value = value.TrimEnd(['.',]);

        const char shortDash = PackUrlPartShortDash;

        var result = new StringBuilder();

        char prev = ' ';
        foreach (char c in (value ?? string.Empty).Trim())
        {
            switch (c)
            {
                case '+':
                case '&':
                    if (prev != shortDash)
                    {
                        result.Append(shortDash);
                    }
                    result.Append("and").Append(shortDash);
                    prev = shortDash;
                    break;

                case ' ':
                case '/':
                case '–':
                case '~':
                case shortDash:
                    if (prev != shortDash)
                        result.Append(shortDash);
                    prev = shortDash;
                    break;

                case 'é':
                    result.Append('e');
                    break;
                case '[':
                case ']':
                case '@':
                case '!':
                case '$':
                case '(':
                case ')':
                case '_':
                case '.':
                    result.Append(c);
                    prev = c;
                    break;

                default:
                    if (char.IsLetterOrDigit(c))
                    {
                        result.Append(c);
                        prev = c;
                    }
                    break;
            }
        }

        return result.ToString().ToLower().Trim(shortDash);
    }

    public static string PackUrlPartOrDefault(string value, char @default = PackUrlPartShortDash)
    {
        var packedValue = PackUrlPart(value);

        if (packedValue.Length == 0)
        {
            return @default.ToString();
        }

        return packedValue;
    }

    public static string TitleCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var stringBuilder = new StringBuilder();
        var tokens = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var added = false;
        for (var i = 0; i < tokens.Length; i++)
        {
            if (added)
            {
                stringBuilder.Append(' ');
            }
            else
            {
                added = true;
            }

            var token = tokens[i];

            if (token.Length > 1 && !RomanNumeral.IsRomanNumeral(token) && !IsArticleOrCoordinatingConjunctionOrPreposition(token))
            {
                var tokenInLowerCase = token.ToLower();

                switch (tokenInLowerCase) //handle edge cases
                {
                    case "kwazulu-natal":
                        stringBuilder.Append("KwaZulu-Natal");
                        break;
                    case "kwazulu":
                        stringBuilder.Append("KwaZulu");
                        break;

                    default:
                        stringBuilder.Append(char.ToUpper(token[0]));
                        stringBuilder.Append(tokenInLowerCase.Substring(1));
                        break;
                }
            }
            else
            {
                stringBuilder.Append(token);
            }
        }

        return stringBuilder.ToString();
    }

    private static bool IsArticleOrCoordinatingConjunctionOrPreposition(string word)
    {
        word = word.ToLower();

        if (_articles.Contains(word))
        {
            return true;
        }

        if (_coordinatingConjunctions.Contains(word))
        {
            return true;
        }

        var prepositions = new[] { "in", "to", };

        return prepositions.Contains(word);
    }

    private static readonly string[] _articles = { "a", "an", "the", };
    private static readonly string[] _coordinatingConjunctions = { "and", "as", "but", "for", "if", "nor", "or", "so", "yet", };

    public static string SentenceCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var newSentence = value.TrimStart();
        var offset = value.Length - newSentence.Length;

        if (newSentence.Length <= 1)
        {
            return value.ToUpper();
        }

        return string.Concat(value.AsSpan(0, offset), value[offset].ToString().ToUpper(), value[(offset + 1)..].ToLower());
    }

    public static string AsAge(TimeSpan age, string zeroLengthText)
    {
        if (age.TotalDays >= 1)
        {
            var days = (int)age.TotalDays;
            if (days == 1)
            {
                return days.ToString() + " day";
            }

            return days.ToString() + " days";
        }
        else if (age.TotalHours >= 1)
        {
            var hours = (int)age.TotalHours;
            if (hours == 1)
            {
                return hours.ToString() + " hour";
            }

            return hours.ToString() + " hours";
        }
        else if (age.TotalMinutes >= 1)
        {
            var min = (int)age.TotalMinutes;
            if (min == 1)
            {
                return min.ToString() + " minute";
            }

            return min.ToString() + " minutes";
        }
        else if (age.TotalSeconds >= 1)
        {
            var sec = (int)age.TotalSeconds;
            if (sec == 1)
            {
                return sec.ToString() + " second";
            }

            return sec.ToString() + " seconds";
        }

        return zeroLengthText;
    }

    public static string AsAgeAlternative(TimeSpan? age, string zeroLengthText)
    {
        if (age == null || age == TimeSpan.Zero)
        {
            return zeroLengthText;
        }

        if (age < TimeSpan.FromHours(1))
        {
            if (age.Value.Minutes == 1)
            {
                return $"1 minute";
            }
            return $"{age.Value.Minutes} minutes";
        }

        var stringBuilder = new StringBuilder();

        if (age.Value.Days == 1)
        {
            stringBuilder.Append("1 day ");
        }
        else if (age.Value.Days > 1)
        {
            stringBuilder.Append(age.Value.Days.ToString() + " days ");
        }

        if (age.Value.Hours == 1)
        {
            stringBuilder.Append("1 hour");
        }
        else if (age.Value.Hours > 1)
        {
            stringBuilder.Append(age.Value.Hours.ToString() + " hours");
        }

        return stringBuilder.ToString().Trim();
    }

    public static string ToCommaAndString(IEnumerable<string> items)
    {
        return ToCommaAndString(items.ToList());
    }

    public static string ToCommaAndString(List<string> items)
    {
        var builder = new StringBuilder();

        for (var i = 0; i < items.Count; i++)
        {
            if (i == items.Count - 1 && i != 0)
            {
                builder.Append(" and ");
            }
            else if (i != 0)
            {
                builder.Append(", ");
            }

            builder.Append(items[i]);
        }

        return builder.ToString();
    }

    public static string AsPercentage(float percentage)
    {
        return (percentage * 100).ToString("0.00") + "%";
    }

    private static List<string> SplitParagraph(string text)
    {
        var result = new List<string>();
        var pos = 0;

        for (var i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '.':
                case '?':
                case '!':
                    AppendSentence(result, text.Substring(pos, i - pos + 1));
                    pos = i + 1;
                    break;
            }
        }

        if (pos < text.Length)
        {
            AppendSentence(result, text.Substring(pos, text.Length - pos));
        }

        return result;
    }

    private static void AppendSentence(List<string> result, string sentence)
    {
        if (!string.IsNullOrEmpty(sentence))
        {
            result.Add(sentence);
        }
    }

    private static bool ContainsPunctuation(string text)
    {
        foreach (var c in text)
        {
            switch (c)
            {
                case '.':
                case '?':
                case '!':
                    return true;
            }
        }

        return false;
    }

    public static string SentenceCaseIfAllUpperCase(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            if (ContainsPunctuation(text))
            {
                var sentences = SplitParagraph(text);
                for (var i = 0; i < sentences.Count; i++)
                {
                    if (!sentences[i].Any(v => char.IsLower(v)))
                    {
                        sentences[i] = SentenceCase(sentences[i]);
                    }
                }

                return string.Join(string.Empty, sentences).Trim();
            }
            else if (!text.Any(i => char.IsLower(i)))
            {
                return SentenceCase(text);
            }
        }

        return text;
    }

    public static string TitleCaseIfAllUpperCase(string text)
    {
        if (!text.Any(char.IsLower) && text.Length > 0)
        {
            return TitleCase(text);
        }

        return text;
    }

    public static void SplitNames(string fullName, out string firstName, out string surname)
    {
        var names = fullName.Split(new[] { ' ', }, StringSplitOptions.RemoveEmptyEntries);

        firstName = string.Empty;
        surname = string.Empty;

        if (names.Length > 0)
        {
            firstName = names[0];
            if (names.Length > 1)
            {
                surname = string.Join(" ", names.Skip(1));
            }
        }
    }

    public static string PlainTextFromHtml(string html)
    {
        if (!string.IsNullOrEmpty(html))
        {
            return PlainTextFromHtmlRegex().Replace(html, string.Empty).Replace("&nbsp;", string.Empty).Trim();
        }

        return html;
    }

    public static string AsMilliseconds(TimeSpan searchEngineDuration)
    {
        return searchEngineDuration.TotalMilliseconds.ToString("###,###,###,##0.0ms");
    }

    public static string ToFriendlyTimeString(TimeSpan span, int maxElements = 2)
    {
        var sb = new StringBuilder();
        var elems = 0;

        void add(string s, int timespanValue)
        {
            if (elems < maxElements && timespanValue > 0)
            {
                sb.AppendFormat("{0:0}{1} ", timespanValue, s);
                elems++;
            }
        }

        add("d", span.Days);
        add("h", span.Hours);
        add("m", span.Minutes);
        add("s", span.Seconds);
        add("ms", span.Milliseconds);

        if (sb.Length == 0)
        {
            sb.Append('0');
        }

        return sb.ToString().Trim();
    }

    public static string ToRelativeFriendlyTimeString(DateTime dateTime)
    {
        var timespan = new TimeSpan(DateTime.UtcNow.Ticks - dateTime.Ticks);

        var delta = timespan.TotalSeconds;

        if (delta < 60)
        {
            return timespan.Seconds.ToString() + "s";
        }

        if (delta < 3600) // 60 mins * 60 sec
        {
            return timespan.Minutes.ToString() + "m" + (timespan.Seconds > 0 ? " " + timespan.Seconds.ToString() + "s" : string.Empty);
        }

        if (delta < 86400)  // 24 hrs * 60 mins * 60 sec
        {
            return timespan.Hours.ToString() + "h" + (timespan.Minutes > 0 ? " " + timespan.Minutes.ToString() + "m" : string.Empty);
        }

        return timespan.Days.ToString() + "d" + (timespan.Hours > 0 ? " " + timespan.Hours.ToString() + "h" : string.Empty);
    }

    // Via: https://stackoverflow.com/questions/10820273/regex-to-extract-initials-from-name
    public static string GetInitials(string fullname)
    {
        if (string.IsNullOrEmpty(fullname))
            return null;

        var initials = GetInitialsRegex();

        return initials.Replace(fullname, "$1");
    }

    public static string ToStringSeparated(this decimal value)
    {
        return $"{value:### ### ### ### ### ### ### ### ### ### ### ### ###.### ### ### ### ###}";
    }

    public static string ToStringSeparated(this int value)
    {
        return $"{value:### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ### ###}";
    }

    [GeneratedRegex(@"<(.|\n)*?>")]
    private static partial Regex PlainTextFromHtmlRegex();

    [GeneratedRegex(@"(\b[a-zA-Z])[a-zA-Z]* ?")]
    private static partial Regex GetInitialsRegex();
}