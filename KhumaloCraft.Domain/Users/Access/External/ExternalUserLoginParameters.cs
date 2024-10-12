namespace KhumaloCraft.Domain.Users.Access.External;

public class ExternalUserLoginParameters
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public LoginSource LoginSource { get; set; }
}
