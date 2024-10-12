using System.Globalization;

namespace KhumaloCraft.Domain.Structs;

public struct Money
{
    private readonly decimal _amount;

    public Money(decimal amount)
    {
        _amount = amount;
    }

    public readonly decimal Amount => _amount;

    public readonly string ToStringWithDecimals()
    {
        var cultureInfo = new CultureInfo("en-ZA");

        var formatted = _amount.ToString("C2", cultureInfo);

        return formatted.Replace(cultureInfo.NumberFormat.CurrencyDecimalSeparator, ".");
    }

    public readonly string ToStringWithoutDecimals()
    {
        var cultureInfo = new CultureInfo("en-ZA");

        var formatted = _amount.ToString("C0", cultureInfo);

        return formatted.Replace(cultureInfo.NumberFormat.CurrencyDecimalSeparator, ".");
    }

    public override readonly string ToString()
    {
        return ToStringWithoutDecimals();
    }
}
