using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Portal.Models.Profile.Password;

public class ChangePasswordModel
{
    [Required]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; }

    [Required]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }

    [Required]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string ConfirmNewPassword { get; set; }
}