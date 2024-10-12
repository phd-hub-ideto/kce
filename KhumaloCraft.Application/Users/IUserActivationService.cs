using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Application.Users;

public interface IUserActivationService
{
    void Activate(User user);
}