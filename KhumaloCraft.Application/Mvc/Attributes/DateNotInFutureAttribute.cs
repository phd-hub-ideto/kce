using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

public class DateNotInFutureAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dateNow = DateTime.Now;

        var dateProperty = (DateTime)value;

        return dateProperty <= dateNow ? ValidationResult.Success : new ValidationResult("Selected date cannot exceed todays date.");
    }
}