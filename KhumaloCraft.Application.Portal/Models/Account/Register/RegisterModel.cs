using KhumaloCraft.Application.Mvc.Attributes;
using KhumaloCraft.Application.Validation.User;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Portal.Models.Account.Register;

public class RegisterModel : IUserRegister
{
    [Required]
    [StringLength(100)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailValidator(EmailValidatorMessageFormat.Short)]
    [Display(Name = "Email Address")]
    public string Username { get; set; }

    private string _mobileNumber;

    [PhoneNumberValidator]
    public string MobileNumber
    {
        get { return string.IsNullOrWhiteSpace(_mobileNumber) ? null : _mobileNumber; }
        set { _mobileNumber = value; }
    }

    [Required]
    public string Password { get; set; }

    [Required]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }

    public string ReturnUrl { get; set; }
}