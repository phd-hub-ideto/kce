using KhumaloCraft.Application.Mvc.Attributes;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Portal.Models.Profile.Password;

public class ActivateCreatePasswordAndSignInRequest : IValidatableObject
{
    [EmailValidator(EmailValidatorMessageFormat.ReferenceField)]
    public string Username { get; set; }
    public string Token { get; set; }
    public string RequestHash { get; set; }
    public string ExpiryDate { get; set; }
    public string ReturnUrl { get; set; }

    [Required(ErrorMessage = "New Password was not supplied")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Confirmation Password was not supplied")]
    public string ConfirmPassword { get; set; }
    public bool RememberMe { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.Equals(NewPassword, ConfirmPassword))
        {
            yield return new ValidationResult("Password does not match confirmation password.", new List<string>());
        }
    }
}