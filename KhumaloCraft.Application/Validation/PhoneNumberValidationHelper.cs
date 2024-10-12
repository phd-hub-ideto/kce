using PhoneNumbers;
using System.Text.RegularExpressions;

namespace KhumaloCraft.Application.Validation;

public static class PhoneNumberValidationHelper
{
    //http://stackoverflow.com/a/1559788/1641076
    //^             // the start of the string
    //(?=.*[a-z])   // use positive look ahead to see if at least one lower case letter exists
    //(?=.*[A-Z])   // use positive look ahead to see if at least one upper case letter exists
    //((?=.*\d)     // use positive look ahead to see if at least one digit exists, open parentheses for grouping
    // |            // use or operator, want to match symbol or number
    //(?=.*[_\W]))  // use positive look ahead to see if at least one underscore or non-word character exists, close parentheses for grouping
    //.+            // gobble up the entire string
    //$             // the end of the string

    public const string PhoneNumberPattern = @"\b^0(\d{9}$)\b|\b^0(\d{10}$)\b";

    private static PhoneNumberUtil _phoneNumberUtil = PhoneNumberUtil.GetInstance();

    public static bool IsValid(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return false;
        }

        return IsValidSimple(phoneNumber) && IsValidAdvance(phoneNumber);
    }

    private static bool IsValidSimple(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, PhoneNumberPattern);
    }

    private static bool IsValidAdvance(string phoneNumber)
    {
        PhoneNumber number;

        if (_phoneNumberUtil.IsPossibleNumber(phoneNumber, "ZA"))
        {
            var normalized = PhoneNumberUtil.Normalize(phoneNumber);
            number = _phoneNumberUtil.Parse(normalized, "ZA");
            return _phoneNumberUtil.IsValidNumber(number);
        }
        return false;
    }
}