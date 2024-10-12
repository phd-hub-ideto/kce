namespace KhumaloCraft.Domain.Users.Access;

public class RegistrationParameters
{
    public string Username { get; set; }
    public string Password { get; set; }
    public PassThroughParameters PassThroughParameters { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MobileNumber { get; set; }
    public bool NoPasswordRegistration { get; set; }
}