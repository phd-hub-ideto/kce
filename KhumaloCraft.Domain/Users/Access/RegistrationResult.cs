using System.ComponentModel;

namespace KhumaloCraft.Domain.Users.Access;

public class RegistrationResult
{
    public bool Success { get; set; }
    public int? UserId { get; set; }
    public List<RegistrationError> Errors { get; set; }
}

public enum RegistrationError
{
    [Description("Invalid First or Last name")]
    InvalidFirstNameOrLastName = 1,
    [Description("Invalid Password")]
    InvalidPassword = 2,
    [Description("User already exists")]
    UserAlreadyExists = 3,
    [Description("Invalid email address")]
    InvalidEmailAddress = 4,
    [Description("Phone Number cannot be used")]
    InvalidPhoneNumber = 5
}