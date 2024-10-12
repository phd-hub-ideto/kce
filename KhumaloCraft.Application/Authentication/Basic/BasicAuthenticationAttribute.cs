using Microsoft.AspNetCore.Authorization;

namespace KhumaloCraft.Application.Authentication.Basic;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class BasicAuthenticationAttribute : AuthorizeAttribute
{
    public BasicAuthenticationAttribute()
    {
        AuthenticationSchemes = BasicAuthenticationHandler.AuthenticationScheme;
    }
}