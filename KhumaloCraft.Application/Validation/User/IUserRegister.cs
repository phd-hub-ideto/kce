namespace KhumaloCraft.Application.Validation.User;

public interface IUserRegister
{
    string Username { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string Password { get; set; }
}