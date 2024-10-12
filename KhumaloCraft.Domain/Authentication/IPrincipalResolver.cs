using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Domain.Security;

public interface IPrincipalResolver
{
    int? GetUserId();
    void SetPrincipal(KhumaloCraftPrincipal khumaloCraftPrincipal);
    bool TryResolveCurrentPrincipal(out KhumaloCraftPrincipal khumaloCraftPrincipal);
    KhumaloCraftPrincipal ResolveCurrentPrincipal();
    void ClearPrincipal();
    int GetRequiredUserId();
    bool TryGetUserId(out int userId);
    bool IsAuthenticated();
    bool TryResolveCurrentUser(out User user);
    User ResolveCurrentUser();
}