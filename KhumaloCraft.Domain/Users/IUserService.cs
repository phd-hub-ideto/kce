namespace KhumaloCraft.Domain.Users;

public interface IUserService
{
    void UpdatePassword(User user, string password);
    void UpdateUser(User user);
    User FetchById(int id);
}