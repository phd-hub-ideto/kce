using KhumaloCraft.Application.Mvc.Attributes;
using KhumaloCraft.Application.Validation.User;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Portal.Models.Profile;

public class ProfileModel : IUserProfile
{
    [Required]
    [MaxLength(100)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    private string _mobileNumber;

    [PhoneNumberValidator]
    public string MobileNumber
    {
        get { return string.IsNullOrWhiteSpace(_mobileNumber) ? null : _mobileNumber; }
        set { _mobileNumber = value; }
    }

    public string Username { get; set; } //read-only field
}