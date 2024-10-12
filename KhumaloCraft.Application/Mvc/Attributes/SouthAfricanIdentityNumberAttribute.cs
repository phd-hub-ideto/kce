using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace KhumaloCraft.Application.Mvc.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SouthAfricanIdentityNumberAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return true; //empty values should be caught using [Required]
        }

        if (value is string stringValue)
        {
            if (stringValue.Length != 13 ||
                !stringValue.Any(i => char.IsDigit(i)) ||
                !DateTime.TryParseExact(stringValue.Substring(0, 6), "yyMMdd", CultureInfo.GetCultureInfo("en-ZA"), DateTimeStyles.AssumeLocal, out var _))
            {
                return false;
            }

            var checksum = 0;
            var multiplier = 1;

            foreach (var digit in stringValue)
            {
                var tempTotal = int.Parse(digit.ToString()) * multiplier;
                if (tempTotal > 9)
                {
                    tempTotal = int.Parse(tempTotal.ToString().Substring(0, 1)) + int.Parse(tempTotal.ToString().Substring(1, 1));
                }
                checksum += tempTotal;
                multiplier = (multiplier % 2 == 0) ? 1 : 2;
            }

            if ((checksum % 10) != 0)
            {
                return false;
            }

            return true;
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format("{0} is not a valid RSA ID number.", name);
    }
}
