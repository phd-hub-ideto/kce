using System.Globalization;
using System.Text;

namespace KhumaloCraft.Shared;

/// <summary>
/// Formatting examples
/// Date        : 01/01/2020
/// DateTime    : 01/01/2020 23:00
/// Rand        : R 1 111.11
/// Rand        : -R 1 111.11
/// Number      : 1 111.11
/// </summary>
public static partial class Formatting
{
    public static readonly string DateFormat = "dd/MM/yyyy";
    public static readonly string DateTimeFormat = $"{DateFormat} HH:mm";

    internal static readonly NumberFormatInfo NumberFormatInfo = new NumberFormatInfo()
    {
        NumberDecimalSeparator = ".",
        NumberGroupSeparator = " ",
        NumberGroupSizes = [3],
        CurrencyDecimalDigits = 2,
        CurrencyGroupSeparator = " ",
        CurrencyGroupSizes = [3],
        CurrencySymbol = "R",
        CurrencyDecimalSeparator = ".",
        CurrencyPositivePattern = 2,
        CurrencyNegativePattern = 9,

    };

    internal static readonly DateTimeFormatInfo DateTimeFormatInfo = new DateTimeFormatInfo()
    {
        DateSeparator = "/",
        TimeSeparator = ":",
    };

    private const string IntegralNumberFormat = "#,###;-#,###;0";

    public static bool TryParseDateString(string dateString, out DateTime date)
    {
        return DateTime.TryParseExact(dateString, DateFormat, DateTimeFormatInfo, DateTimeStyles.None, out date);
    }

    public static bool TryParseDateTimeString(string dateTimeString, out DateTime dateTime)
    {
        return DateTime.TryParseExact(dateTimeString, DateTimeFormat, DateTimeFormatInfo, DateTimeStyles.None, out dateTime);
    }

    public static string ToDateString(this DateTime dateTime)
    {
        return dateTime.ToString(DateFormat);
    }

    public static string ToDateTimeString(this DateTime dateTime)
    {
        return dateTime.ToString(DateTimeFormat);
    }

    public static string ToString(int value)
    {
        return value.ToString(IntegralNumberFormat, NumberFormatInfo);
    }
    public static string ToString(long value)
    {
        return value.ToString(IntegralNumberFormat, NumberFormatInfo);
    }
    public static string ToString(short value)
    {
        return value.ToString(IntegralNumberFormat, NumberFormatInfo);
    }

    public static string ToRandString(int value)
    {
        return value.ToString("C0", NumberFormatInfo);
    }
    public static string ToRandString(long value)
    {
        return value.ToString("C0", NumberFormatInfo);
    }

    public static string ToRandString(decimal value, bool truncateDecimal = false)
    {
        if (truncateDecimal)
            return value.ToString("C0", NumberFormatInfo);

        return value.ToString("C2", NumberFormatInfo);
    }
    public static string ToRandString(double value, bool truncateDecimal = false)
    {
        if (truncateDecimal)
            return value.ToString("C0", NumberFormatInfo);

        return value.ToString("C2", NumberFormatInfo);
    }
    public static string ToRandString(float value, bool truncateDecimal = false)
    {
        if (truncateDecimal)
            return value.ToString("C0", NumberFormatInfo);

        return value.ToString("C2", NumberFormatInfo);
    }

    public struct Unit
    {
        public Unit(string longName, string shortName, double size)
        {
            LongName = longName;
            ShortName = shortName;
            Size = size;
        }

        public string LongName, ShortName;
        public double Size;
    }

    private const int ByteUnitBase = 1024;
    public static readonly Unit[] ByteUnits = [
        new Unit("byte", "B", Math.Pow(ByteUnitBase,0)),
        new Unit("kilobyte", "KB", Math.Pow(ByteUnitBase,1)),
        new Unit("megabyte", "MB", Math.Pow(ByteUnitBase,2)),
        new Unit("gigabyte", "GB", Math.Pow(ByteUnitBase,3)),
        new Unit("terabyte", "TB", Math.Pow(ByteUnitBase,4)),
        new Unit("petabyte", "PB", Math.Pow(ByteUnitBase,5)),
        new Unit("exabyte", "EB", Math.Pow(ByteUnitBase,6)),
        new Unit("zettabyte", "ZB", Math.Pow(ByteUnitBase,7)),
        new Unit("yottabyte", "YB", Math.Pow(ByteUnitBase,8)),
    ];

    public enum SuffixType
    {
        Full,
        Abbreviation
    }

    public static string BytesToString(long byteCount, SuffixType suffixType = SuffixType.Abbreviation, bool plural = true)
    {
        var sb = new StringBuilder();
        var bu = ByteUnits[0];
        foreach (var b in ByteUnits)
        {
            if (byteCount < b.Size)
            {
                break;
            }
            bu = b;
        }
        var d = MathHelpers.TruncateDecimal(byteCount / (decimal)bu.Size, 2);

        sb.Append((d == 0) ? "0" : d.ToString("#.##"));
        sb.Append(' ');

        if (suffixType == SuffixType.Abbreviation)
        {
            sb.Append(bu.ShortName);
        }
        else
        {
            sb.Append(bu.LongName);
        }

        if (plural && (d != 1))
        {
            sb.Append('s');
        }
        return sb.ToString();
    }

    public const int BitUnitBase = 1000;
    public static readonly Unit[] BitUnits = [
        new Unit("bit", "b", Math.Pow(BitUnitBase,0)),
        new Unit("kilobit", "Kb", Math.Pow(BitUnitBase,1)),
        new Unit("megabit", "Mb", Math.Pow(BitUnitBase,2)),
        new Unit("gigabit", "Gb", Math.Pow(BitUnitBase,3)),
        new Unit("terabit", "Tb", Math.Pow(BitUnitBase,4)),
        new Unit("petabit", "Pb", Math.Pow(BitUnitBase,5)),
        new Unit("exabit", "Eb", Math.Pow(BitUnitBase,6)),
        new Unit("zettabit", "Zb", Math.Pow(BitUnitBase,7)),
        new Unit("yottabit", "Yb", Math.Pow(BitUnitBase,8)),
    ];

    public static string BitsToString(
        long bitCount,
        SuffixType suffixType = SuffixType.Abbreviation,
        bool plural = true)
    {
        var sb = new StringBuilder();
        var bu = BitUnits[0];

        foreach (var b in BitUnits)
        {
            if (bitCount < b.Size)
            {
                break;
            }
            bu = b;
        }

        var d = MathHelpers.TruncateDecimal(bitCount / (decimal)bu.Size, 2);

        sb.Append((d == 0) ? "0" : d.ToString("#.##"));
        sb.Append(' ');

        if (suffixType == SuffixType.Abbreviation)
        {
            sb.Append(bu.ShortName);
        }
        else
        {
            sb.Append(bu.LongName);
        }

        if (plural && (d != 1))
        {
            sb.Append('s');
        }

        return sb.ToString();
    }
}