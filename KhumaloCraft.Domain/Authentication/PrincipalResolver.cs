using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Domain.Authentication;

public abstract class PrincipalResolver : IPrincipalResolver
{
    public static IPrincipalResolver Instance => DependencyManager.Container.GetInstance<IPrincipalResolver>();

    public abstract void ClearPrincipal();
    public abstract void SetPrincipal(KhumaloCraftPrincipal khumaloCraftPrincipal);
    public abstract bool TryResolveCurrentPrincipal(out KhumaloCraftPrincipal khumaloCraftPrincipal);

    public int GetRequiredUserId()
    {
        return ResolveCurrentPrincipal().UserId;
    }

    public int? GetUserId()
    {
        if (TryResolveCurrentPrincipal(out var khumaloCraftPrincipal))
        {
            return khumaloCraftPrincipal.UserId;
        }

        return null;
    }

    public bool IsAuthenticated()
    {
        return TryResolveCurrentPrincipal(out _);
    }

    public KhumaloCraftPrincipal ResolveCurrentPrincipal()
    {
        if (TryResolveCurrentPrincipal(out var khumaloCraftPrincipal))
        {
            return khumaloCraftPrincipal;
        }
        else
        {
            throw new Exception("Current principal could not be resolved.");
        }
    }

    public User ResolveCurrentUser()
    {
        return ResolveCurrentPrincipal().User;
    }

    public bool TryGetUserId(out int userId)
    {
        if (TryResolveCurrentPrincipal(out var khumaloCraftPrincipal))
        {
            userId = khumaloCraftPrincipal.UserId;
            return true;
        }

        userId = default;

        return false;
    }

    public bool TryResolveCurrentUser(out User user)
    {
        if (TryResolveCurrentPrincipal(out var khumaloCraftPrincipal))
        {
            user = khumaloCraftPrincipal.User;

            return true;
        }

        user = default;

        return false;
    }
}