namespace KhumaloCraft.Shared;

public static class MathHelpers
{
    public static decimal TruncateAtDigit(decimal value, int digit)
    {
        if (digit < -28 || digit > 29)
            throw new ArgumentOutOfRangeException(nameof(digit), "A decimal can only have 29 integral digits and 28 fractional digits.");

        decimal lfactor = (decimal)Math.Pow(10, digit);

        return Math.Truncate(value / lfactor) * lfactor;
    }

    public static decimal TruncateDecimal(decimal value, int decimalPlaces)
    {
        return TruncateAtDigit(value, -decimalPlaces);
    }
    public static decimal TruncateDecimal(double value, int decimalPlaces)
    {
        return TruncateDecimal((decimal)value, decimalPlaces);
    }
    public static decimal TruncateDecimal(float value, int decimalPlaces)
    {
        return TruncateDecimal((decimal)value, decimalPlaces);
    }
}