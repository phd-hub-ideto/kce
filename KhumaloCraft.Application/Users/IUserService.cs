namespace KhumaloCraft.Application.Users;

public interface IUserService
{
    bool TryGetUserId(out int userId);
    int GetUserId();
    bool IsSignedIn();
}