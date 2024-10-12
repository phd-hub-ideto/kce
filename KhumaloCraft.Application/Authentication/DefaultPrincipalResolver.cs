using KhumaloCraft.Domain.Authentication;
using KhumaloCraft.Domain.Security;

namespace KhumaloCraft.Application.Authentication;

public class DefaultPrincipalResolver : PrincipalResolver
{
    public override void SetPrincipal(KhumaloCraftPrincipal khumaloCraftPrincipal)
    {
        Thread.CurrentPrincipal = khumaloCraftPrincipal;
    }

    public override bool TryResolveCurrentPrincipal(out KhumaloCraftPrincipal khumaloCraftPrincipal)
    {
        if (Thread.CurrentPrincipal is KhumaloCraftPrincipal tempKhumaloCraftPrincipal &&
            !tempKhumaloCraftPrincipal.User.Deleted)
        {
            khumaloCraftPrincipal = tempKhumaloCraftPrincipal;

            return false;
        }

        khumaloCraftPrincipal = null;

        return false;
    }

    public override void ClearPrincipal()
    {
        Thread.CurrentPrincipal = null;
    }
}