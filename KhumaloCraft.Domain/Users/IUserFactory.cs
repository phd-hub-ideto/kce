namespace KhumaloCraft.Domain.Users;

public interface IUserFactory
{
    User CreateNonSignInUser(string emailAddress);
    User CreateSignInUser(
        string emailAddress, string password, string firstName,
        string lastName, string mobileNumber);
}