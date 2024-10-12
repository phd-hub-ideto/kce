using KhumaloCraft.Dependencies;

namespace KhumaloCraft.Domain.Users;

[Singleton]
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFactory _userFactory;

    public UserService(
        IUserRepository userRepository,
        IUserFactory userFactory)
    {
        _userRepository = userRepository;
        _userFactory = userFactory;
    }

    public User FetchById(int id)
    {
        return _userRepository.Query().Single(u => u.Id == id);
    }

    public void UpdatePassword(User user, string password)
    {
        user.SetPassword(password);

        _userRepository.Upsert(user);
    }

    /// <summary>
    /// Only use this method for Profile updates.
    /// </summary>
    /// <param name="user"></param>
    public void UpdateUser(User user)
    {
        _userRepository.Upsert(user);
    }
}