using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Application.Users;

[Singleton(Contract = typeof(IUserFactory))]
public class UserFactory : IUserFactory
{
    private readonly IUserRepository _userRepository;

    public UserFactory(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User CreateNonSignInUser(string emailAddress)
    {
        if (emailAddress == null)
        {
            throw new ArgumentNullException(nameof(emailAddress));
        }

        if (_userRepository.ExistsByUsername(emailAddress))
        {
            throw new Exception($"A user with the email address '{emailAddress}' already exists.");
        }

        var user = new User(emailAddress);

        _userRepository.Upsert(user);

        return user;
    }

    public User CreateSignInUser(
        string emailAddress, string password, string firstName,
        string lastName, string mobileNumber)
    {
        if (emailAddress == null)
        {
            throw new ArgumentNullException(nameof(emailAddress));
        }

        if (_userRepository.ExistsByUsername(emailAddress))
        {
            throw new Exception($"A user with the email address '{emailAddress}' already exists.");
        }

        throw new NotImplementedException();
    }
}