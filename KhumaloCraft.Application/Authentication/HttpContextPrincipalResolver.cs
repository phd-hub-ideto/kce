using KhumaloCraft.Domain.Security;
using Microsoft.AspNetCore.Http;
using KhumaloCraft.Domain.Authentication;

namespace KhumaloCraft.Application.Authentication;

public class HttpContextPrincipalResolver : PrincipalResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextPrincipalResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override void SetPrincipal(KhumaloCraftPrincipal khumaloCraftPrincipal)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (khumaloCraftPrincipal != null)
        {
            httpContext.User = khumaloCraftPrincipal;
        }
        else
        {
            httpContext.User = null;
        }
    }

    public override bool TryResolveCurrentPrincipal(out KhumaloCraftPrincipal khumaloCraftPrincipal)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.User is not null &&
            httpContext.User is KhumaloCraftPrincipal tempKhumaloCraftPrincipal &&
            !tempKhumaloCraftPrincipal.User.Deleted)
        {
            khumaloCraftPrincipal = tempKhumaloCraftPrincipal;

            return true;
        }

        khumaloCraftPrincipal = null;

        return false;
    }

    public override void ClearPrincipal()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        httpContext.User = null;
    }
}