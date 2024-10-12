using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Application.UserContext;

public class PersonalDetails
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }

    private string _mobileNumber;

    public string MobileNumber
    {
        get
        {
            return _mobileNumber ?? string.Empty;
        }
        set
        {
            _mobileNumber = value;
        }
    }

    public PersonalDetails()
    {

    }

    public PersonalDetails(User currentUser)
    {
        FirstName = currentUser.FirstName;
        LastName = currentUser.LastName;
        MobileNumber = currentUser.MobileNumber;
        EmailAddress = currentUser.Username;
    }
}