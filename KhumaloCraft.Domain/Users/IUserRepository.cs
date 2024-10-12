namespace KhumaloCraft.Domain.Users;

public interface IUserRepository
{
    IQueryable<User> Query(bool includeDeleted = false);
    void Upsert(User user);
    void UpdateLastLoginDate(int userId);
    bool ExistsByUsername(string username);
}