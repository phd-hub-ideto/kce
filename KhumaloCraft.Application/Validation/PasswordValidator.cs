using KhumaloCraft.Domain.Security;
using System.Text.RegularExpressions;

namespace KhumaloCraft.Application.Validation;

public static class PasswordValidator
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
    private const string ComplexPasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])((?=.*\d)|(?=.*(_|[^\w]))).+$";

    private static List<string> _invalidPasswords = ["password", "abc123", "passaword", "khumalocraft"];

    public class PasswordComplexityValidationResult
    {
        public PasswordComplexityInValidationReason Status { get; set; }

        public string Message { get; set; }
    }

    public static void ValidateResetPasswordToken(byte[] passwordSalt, string token)
    {
        if (!IsValidResetPasswordToken(passwordSalt, token))
        {
            throw Exceptions.TokenIsInvalid();
        }
    }

    public static bool IsValidResetPasswordToken(byte[] passwordSalt, string token)
    {
        var tokenInfo = token.Split('|');

        return SecurityHelper.GenerateHashForResetPassword(passwordSalt) == tokenInfo[0];
    }

    public static void ValidateResetPasswordToken(
        string username, string token,
        IResetPasswordHasher resetPasswordHasher,
        string returnUrl = null)
    {
        if (!IsValidResetPasswordToken(username, token, resetPasswordHasher, returnUrl))
        {
            throw Exceptions.TokenIsInvalid();
        }
    }

    public static bool IsValidResetPasswordToken(
        string username, string token,
        IResetPasswordHasher resetPasswordHasher, string returnUrl = null)
    {
        return resetPasswordHasher.IsValidToken(username, token, returnUrl);
    }

    private static string GetDefaultPasswordErrorMessage(PasswordComplexityInValidationReason passwordComplexityInValidationReason)
    {
        switch (passwordComplexityInValidationReason)
        {
            case PasswordComplexityInValidationReason.None:
                return "Password is sufficiently complex.";

            case PasswordComplexityInValidationReason.CommonPassword:
                return "Password contains common password phrases and is invalid.";

            case PasswordComplexityInValidationReason.NotComplex:
                return "Password must have at least one upper and one lower case letter, and at least one symbol or number.";

            case PasswordComplexityInValidationReason.TooShort:
                return "Password needs to be at least 6 characters.";

            default:
                throw new Exception($"{nameof(passwordComplexityInValidationReason)} extra case not handled");
        }
    }

    public static PasswordComplexityValidationResult IsPasswordComplex(string password, Func<PasswordComplexityInValidationReason, string> getMessage = null)
    {
        var result = new PasswordComplexityValidationResult();

        var notComplex = false;

        if (getMessage == null)
            getMessage = GetDefaultPasswordErrorMessage;

        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            result.Message = getMessage(PasswordComplexityInValidationReason.TooShort);
            result.Status = PasswordComplexityInValidationReason.TooShort;

            return result;
        }

        if (password.Length <= 25)
        {
            Func<string, string> transform = (_password) =>
            {
                if (_password.Contains("@"))
                {
                    _password = _password.Replace('@', 'a');
                }

                if (_password.Contains("0"))
                {
                    _password = _password.Replace('0', 'o');
                }

                return _password.ToLower();
            };

            var transformedPassword = transform(password);

            foreach (var _invalidPassword in _invalidPasswords)
            {
                if (transformedPassword.Contains(_invalidPassword))
                {
                    result.Message = getMessage(PasswordComplexityInValidationReason.CommonPassword);
                    result.Status = PasswordComplexityInValidationReason.CommonPassword;

                    return result;
                }
            }

            if (!Regex.IsMatch(password, ComplexPasswordPattern, RegexOptions.Compiled))
            {
                notComplex = true;
            }
        }
        else
        {
            notComplex = password.Distinct().Count() < 6;
        }

        if (notComplex)
        {
            result.Message = getMessage(PasswordComplexityInValidationReason.NotComplex);
            result.Status = PasswordComplexityInValidationReason.NotComplex;

            return result;
        }

        result.Message = getMessage(PasswordComplexityInValidationReason.None);
        result.Status = PasswordComplexityInValidationReason.None;

        return result;
    }

    public static class Exceptions
    {
        internal static Exception TokenIsInvalid()
        {
            throw new Exception("Security token is invalid.");
        }
    }
}