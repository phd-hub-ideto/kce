using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Validation.User;

public class UserProfileValidator : IValidator<IUserProfile>
{
    public bool IsValid(IUserProfile userProfile, out ValidationErrors<UserProfileValidationFields> errors)
    {
        errors = new ValidationErrors<UserProfileValidationFields>();

        if (!string.IsNullOrWhiteSpace(userProfile.FirstName) && userProfile.FirstName.Length > 150)
        {
            errors.Add(UserProfileValidationFields.FirstName, "First name cannot exceed 150 characters.");
        }

        if (!string.IsNullOrWhiteSpace(userProfile.LastName) && userProfile.LastName.Length > 150)
        {
            errors.Add(UserProfileValidationFields.LastName, "Last name cannot exceed 150 characters.");
        }

        if (!string.IsNullOrWhiteSpace(userProfile.MobileNumber))
        {
            if (!PhoneNumberValidationHelper.IsValid(userProfile.MobileNumber))
            {
                errors.Add(UserProfileValidationFields.MobileNumber, "Please enter a valid phone number (10 or 11 digit number starting with 0).");
            }
        }

        return !errors.HasErrors();
    }

    public IEnumerable<ValidationResult> Validate(IUserProfile instance)
    {
        if (instance is null)
        {
            yield break;
        }

        if (!string.IsNullOrWhiteSpace(instance.FirstName) && instance.FirstName.Length > 150)
        {
            yield return new ValidationResult("First name cannot exceed 150 characters.", [nameof(IUserProfile.FirstName)]);
        }

        if (!string.IsNullOrWhiteSpace(instance.LastName) && instance.LastName.Length > 150)
        {
            yield return new ValidationResult("Last name cannot exceed 150 characters.", [nameof(IUserProfile.LastName)]);
        }

        if (!string.IsNullOrWhiteSpace(instance.MobileNumber))
        {
            if (!PhoneNumberValidationHelper.IsValid(instance.MobileNumber))
            {
                yield return new ValidationResult("Please enter a valid phone number (10 or 11 digit number starting with 0).", [nameof(IUserProfile.MobileNumber)]);
            }
        }
    }
}