using System.ComponentModel;

namespace KhumaloCraft.Application.Cookies;

// TODO-L: attribute to define duration, httpOnly, etc.
public enum Cookie
{
    [Description("KCU")]
    User,

    [Description("KCUUEYED")]
    UserId,

    [Description("KCUINF")]
    UserInfo,

    [Description("KCUD")]
    UserDetails,

    [Description("KCVINF")]
    VisitInfo,

    [Description("KCEATH")]   // Set in Web.configs
    FormsAuthentication,

    [Description("KCSEYED")] // Set in Web.configs
    SessionId,

    [Description("KCUL")]
    UserLocation,

    [Description("KCUP")]
    UserPreferences,

    [Description("KCPSN")]
    PortalSession,

    [Description("KCATH")]
    AuthenticationCookie,

    [Description("KCACP")]
    AcceptedCookiePolicy
}