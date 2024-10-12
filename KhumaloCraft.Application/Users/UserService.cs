using KhumaloCraft.Domain.Dates;
using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Application.Users;

public class UserService : IUserService
{
    private readonly IDateProvider _dateProvider;
    private readonly IPrincipalResolver _principalResolver;

    public UserService(IDateProvider dateProvider, IPrincipalResolver principalResolver)
    {
        _dateProvider = dateProvider;
        _principalResolver = principalResolver;
    }

    public bool IsSignedIn()
    {
        return _principalResolver.IsAuthenticated();
    }

    public int GetUserId()
    {
        return _principalResolver.GetRequiredUserId();
    }

    public bool TryGetUserId(out int userId)
    {
        if (IsSignedIn())
        {
            userId = _principalResolver.GetRequiredUserId();
            return true;
        }

        userId = -1;

        return false;
    }
}