using KhumaloCraft.Application.Mvc.Attributes;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Portal.Models.Account.Login;

public class LoginModel
{
    [Required]
    [EmailValidator(EmailValidatorMessageFormat.Short)]
    [Display(Name = "Email Address")]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }
    public bool IsEmailLinkExpired { get; set; }
    public bool RememberMe { get; set; } = true;

    public LoginModel()
    {
        Username = Password = string.Empty;
    }
}