namespace KhumaloCraft.Application.Validation;

public class FieldCaseValidator
{
    private const int MaxAllowedUpperCaseCharacters = 5;

    public static bool IsAllUpperCase(string value)
    {
        if (!string.IsNullOrEmpty(value) && value.Trim().Length > MaxAllowedUpperCaseCharacters)
        {
            return !value.Any(char.IsLower);
        }

        return false;
    }
}