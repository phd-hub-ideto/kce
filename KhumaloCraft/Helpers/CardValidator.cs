/*
    Source: https://gist.github.com/d-vs/d2e3554c8053a059c14d43303b14836f
    Author: "d-vs"
 */

namespace KhumaloCraft.Helpers;

public static class CardValidator
{
    public const int MinCardNumberLength = 13;
    public const int MaxCardNumberLength = 19;

    #region Method IsValidNumber

    /// <summary>
    /// Returns a value indicating whether the specified value is a valid credit card number, using a combination of
    /// basic validation and the Luhn algorithm.
    /// </summary>
    /// <param name="number">The number to check.</param>
    public static bool IsValidNumber(string number)
    {
        if (string.IsNullOrEmpty(number) || number.Length < MinCardNumberLength || number.Length > MaxCardNumberLength || number.ContainsNonNumeric())
        {
            return false;
        }

        //Luhn algorithm
        char[] reversed = number.ToCharArray().Reverse().ToArray();

        int checkSum = 0;
        for (int iChar = 0; iChar < reversed.Length; iChar++)
        {
            int charInt = int.Parse(reversed[iChar].ToString());
            if ((iChar + 1) % 2 == 0)
            {
                charInt *= 2;
            }

            char[] intChars = charInt.ToString().ToCharArray();
            foreach (char c in intChars)
            {
                checkSum += int.Parse(c.ToString());
            }
        }

        return checkSum % 10 == 0;
    }

    #endregion

    #region IsValidExpiry

    /// <summary>
    /// Returns a value indicating whether the given year and month strings represent a valid 
    /// expiry date when compared to the current local date.
    /// </summary>
    /// <param name="year">
    /// The year, which needs to be convertable to a System.Int32.
    /// </param>
    /// <param name="month">The month, which needs to be convertable to a System.Int32 between 1 and 12 (inclusive).
    /// </param>
    /// <param name="maxYearsAhead">
    /// The maximum number of years ahead that the year component can be beyond the current year.
    /// </param>
    public static bool IsValidExpiry(string year, string month, int maxYearsAhead)
    {
        if (!int.TryParse(year, out int yearInt) || !int.TryParse(month, out int monthInt))
        {
            return false;
        }
        return IsValidExpiry(yearInt, monthInt, maxYearsAhead);
    }

    /// <summary>
    /// Returns a value indicating whether the given year and month represent a valid 
    /// expiry date when compared to the current local date.
    /// </summary>
    /// <param name="year">
    /// The year, which needs to be convertable to a System.Int32.
    /// </param>
    /// <param name="month">The month, which needs to be convertable to a System.Int32 between 1 and 12 (inclusive).
    /// </param>
    /// <param name="maxYearsAhead">
    /// The maximum number of years ahead that the year component can be beyond the current year.
    /// </param>
    public static bool IsValidExpiry(int year, int month, int maxYearsAhead)
    {
        var now = DateTime.Now;
        if (month < 1 || month > 12 || year < now.Year || year > (now.Year + maxYearsAhead))
        {
            return false;
        }

        //Create a date for the current month to disregard day.
        var currentMonth = new DateTime(now.Year, now.Month, 1);

        var expiry = new DateTime(year, month, 1);
        return expiry.Date >= currentMonth.Date;
    }

    #endregion

    #region Method IsValidCVC

    /// <summary>
    /// Returns a value indicating whether the given CVC number is valid according to whether the card is of type American Express.
    /// </summary>
    /// <param name="cvc">The CVC value to check.</param>
    /// <param name="isAMEX">Whether the card is of type American Express.</param>
    public static bool IsValidCVC(string cvc, bool isAMEX)
    {
        return IsValidCVC(cvc, isAMEX, out int requiredDigits);
    }

    /// <summary>
    /// Returns a value indicating whether the given CVC number is valid according to whether the card is of type American Express.
    /// </summary>
    /// <param name="cvc">The CVC value to check.</param>
    /// <param name="isAMEX">Whether the card is of type American Express.</param>
    /// <param name="requiredDigits">Out parameter returning the number of required digits.</param>
    public static bool IsValidCVC(string cvc, bool isAMEX, out int requiredDigits)
    {
        requiredDigits = isAMEX ? 4 : 3;
        if (string.IsNullOrEmpty(cvc) || cvc.ContainsNonNumeric())
        {
            return false;
        }

        return cvc.Length == requiredDigits;
    }

    #endregion

}