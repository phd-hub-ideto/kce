using KhumaloCraft.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PhoneNumberValidatorAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        string valueString = value as string;

        if (string.IsNullOrWhiteSpace(valueString))
            return true;    // empty values should be caught using [Required]

        return PhoneNumberValidationHelper.IsValid(valueString);
    }

    public override string FormatErrorMessage(string name)
    {
        return "Please enter a valid phone number (10 or 11 digit number starting with 0).";
    }
}
