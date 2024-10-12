using KhumaloCraft.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

public class NotAllUpperCaseAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value != null && FieldCaseValidator.IsAllUpperCase(value.ToString()))
        {
            return new ValidationResult(string.Format("{0} cannot be in upper case", validationContext.DisplayName));
        }

        return ValidationResult.Success;
    }
}