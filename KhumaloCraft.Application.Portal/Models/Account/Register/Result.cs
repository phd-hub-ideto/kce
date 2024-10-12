namespace KhumaloCraft.Application.Portal.Models.Account.Register;

public enum Result
{
    InvalidUsername = 1,
    InvalidPassword = 2,
    ExistingUser = 3,
    UnacceptedTermsAndConditions = 4,
    InvalidFirstName = 5,
    InvalidLastName = 6,
    InvalidMobileNumber = 7
}