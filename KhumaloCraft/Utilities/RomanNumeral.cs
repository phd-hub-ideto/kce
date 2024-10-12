namespace KhumaloCraft.Utilities;

public static class RomanNumeral
{
    public static bool IsRomanNumeral(string romanNumeral)
    {
        return TryParseRomanNumeral(romanNumeral, out _, out _);
    }

    public static bool TryParseRomanNumeral(string romanNumeral, out int value)
    {
        return TryParseRomanNumeral(romanNumeral, out value, out _);
    }

    public static int ParseRomanNumeral(string romanNumeral)
    {
        if (!TryParseRomanNumeral(romanNumeral, out var result, out var error))
        {
            throw new Exception(error);
        }

        return result;
    }

    private enum RomanLetter
    {
        I = 1,
        V = 5,
        X = 10,
        L = 50,
        C = 100,
        D = 500,
        M = 1000
    }

    private static Dictionary<char, RomanLetter> _romanCharacters = new Dictionary<char, RomanLetter>()
    {
        {'I', RomanLetter.I},
        {'V', RomanLetter.V},
        {'X', RomanLetter.X},
        {'L', RomanLetter.L},
        {'C', RomanLetter.C},
        {'D', RomanLetter.D},
        {'M', RomanLetter.M},
    };

    public static bool IsRomalLetter(char letter)
    {
        return letter switch
        {
            'I' or 'i' or 'V' or 'v' or 'X' or 'x' or 'L' or 'l' or 'C' or 'c' or 'D' or 'd' or 'M' or 'm' => true,
            _ => false,
        };
    }

    private const string RuleA = "Inavlid roman numeral. Invalid roman numeral. A single letter may only be repeated up to three times consecutively with each occurrence of the value being additive.";
    private const string RuleB = "Invalid roman numeral. A small-value numeral may only be placed to the left of a larger value.";
    private const string RuleC = "Inavlid roman numeral. Numerals representing numbers beginning with a '5' (V, L and D) may only appear once in each roman numeral.";
    private const string RuleD = "Inavlid roman numeral. The size of value of each the numeral as read from left to right must never increase from one letter to the next.";

    public static bool TryParseRomanNumeral(string romanNumeral, out int value, out string errorMessage)
    {

        //ensure it is is a trimmed string
        romanNumeral = (romanNumeral ?? string.Empty).Trim();

        if (romanNumeral.Length == 0)
        {
            errorMessage = null;
            value = -1;
            return false;
        }

        //handle zero
        if ((romanNumeral.Length == 1) && ((romanNumeral[0] == 'N') || (romanNumeral[0] == 'n')))
        {
            errorMessage = null;
            value = 0;
            return true;
        }

        //first check and see if it contains only roman letters
        foreach (char letter in romanNumeral)
        {
            if (!IsRomalLetter(letter))
            {
                value = -1;
                errorMessage = null;
                return false;
            }
        }

        //now that we know it is potentially a valid roman numeral, uppercase to simplify comparisons
        romanNumeral = romanNumeral.ToUpper();

        if (romanNumeral.Split('V').Length > 2 ||
            romanNumeral.Split('L').Length > 2 ||
            romanNumeral.Split('D').Length > 2)
        {
            value = -1;
            errorMessage = RuleC;
            return false;
        }

        int iCount = 1;
        char cLast = 'Z';
        foreach (char cNumeral in romanNumeral)
        {
            if (cNumeral == cLast)
            {
                iCount++;
                if (iCount == 4)
                {
                    value = -1;
                    errorMessage = RuleA;
                    return false;
                }
            }
            else
            {
                iCount = 1;
                cLast = cNumeral;
            }
        }

        int ptr = 0;
        int maxDigit = 1000;
        List<int> values = new List<int>();

        while (ptr < romanNumeral.Length)
        {

            char numeral = romanNumeral[ptr];
            int digit = (int)_romanCharacters[numeral];

            //first digit
            if (digit > maxDigit)
            {
                value = -1;
                errorMessage = RuleB;
                return false;
            }

            //next digit
            int nextDigit = 0;
            if (ptr < romanNumeral.Length - 1)
            {
                char nextNumeral = romanNumeral[ptr + 1];
                nextDigit = (int)_romanCharacters[nextNumeral];

                if (nextDigit > digit)
                {
                    switch (numeral)
                    {
                        case 'I':
                        case 'X':
                        case 'C':
                            if (nextDigit > (digit * 10) || romanNumeral.Split(numeral).Length > 3)
                            {
                                value = -1;
                                errorMessage = RuleB;
                                return false;
                            }
                            break;
                        default:
                            value = -1;
                            errorMessage = RuleB;
                            return false;
                    }
                    maxDigit = digit - 1;
                    digit = nextDigit - digit;
                    ptr++;
                }
            }

            values.Add(digit);

            // Next digit
            ptr++;
        }

        // Rule 5
        for (int i = 0; i < values.Count - 1; i++)
        {
            if (values[i] < values[i + 1])
            {
                value = -1;
                errorMessage = RuleD;
                return false;
            }
        }

        // Rule 2
        int total = 0;
        foreach (int digit in values)
        {
            total += digit;
        }

        value = total;
        errorMessage = null;
        return true;
    }
}