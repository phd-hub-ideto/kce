using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

namespace KhumaloCraft.Application.Authentication;

internal class AuthenticationCookieTicketFormat : SecureDataFormat<AuthenticationTicket>
{
    //DO NOT change this unless you have a very good reason to do so.
    //Changing this will result in the data protector being unable to unprotect existing cookies.
    public const string CookieDataProtectionPurpose = nameof(CookieDataProtectionPurpose);

    public AuthenticationCookieTicketFormat(
        IDataProtector dataProtector,
        IHttpContextProvider httpContextProvider,
        IUserRepository userRepository,
        IUserRolePermissionRepository userRolePermissionRepository,
        IUserRoleRepository userRoleRepository)
        : base(
            new TicketSerializer(
                httpContextProvider,
                userRepository,
                userRolePermissionRepository,
                userRoleRepository),
            dataProtector)
    { }

    private class TicketSerializer : IDataSerializer<AuthenticationTicket>
    {
        private readonly IHttpContextProvider _httpContextProvider;
        private readonly IUserRepository _userRepository;
        private readonly IUserRolePermissionRepository _userRolePermissionRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private const short CurrentCookieVersion = 2;

        public TicketSerializer(IHttpContextProvider httpContextProvider, IUserRepository userRepository, IUserRolePermissionRepository userRolePermissionRepository, IUserRoleRepository userRoleRepository)
        {
            _httpContextProvider = httpContextProvider;
            _userRepository = userRepository;
            _userRolePermissionRepository = userRolePermissionRepository;
            _userRoleRepository = userRoleRepository;
        }

        public AuthenticationTicket Deserialize(byte[] data)
        {
            KhumaloCraftPrincipal khumaloCraftPrincipal;

            var authenticationCookieContainer = JsonSerializer.Deserialize<AuthenticationCookieContainer>(data);

            var authenticationProperties = new AuthenticationProperties();

            switch (authenticationCookieContainer.Version)
            {
                case 1:
                    var authCookieUserData = JsonSerializer.Deserialize<AuthenticationCookieUserData>(authenticationCookieContainer.AuthCookie);

                    User user = _userRepository.Query(includeDeleted: true).Single(u => u.Id == authCookieUserData.UserId);

                    khumaloCraftPrincipal = KhumaloCraftPrincipal.Create(user, _userRolePermissionRepository, _userRoleRepository);

                    _httpContextProvider.SetItem(AuthenticationCookieUserData.AuthCookieUserDataKey, authCookieUserData);
                    break;
                case 2:
                    var authCookieUserDataV2 = JsonSerializer.Deserialize<AuthenticationCookieUserData>(authenticationCookieContainer.AuthCookie);

                    User userV2 = _userRepository.Query(includeDeleted: true).Single(u => u.Id == authCookieUserDataV2.UserId);

                    khumaloCraftPrincipal = KhumaloCraftPrincipal.Create(userV2, _userRolePermissionRepository, _userRoleRepository);

                    _httpContextProvider.SetItem(AuthenticationCookieUserData.AuthCookieUserDataKey, authCookieUserDataV2);

                    authenticationProperties.IssuedUtc = authCookieUserDataV2.IssuedUtc;
                    authenticationProperties.ExpiresUtc = authCookieUserDataV2.ExpiresUtc;
                    authenticationProperties.IsPersistent = authCookieUserDataV2.IsPersistent;
                    break;
                default:
                    throw new NotSupportedException($"Authentication cookie version {authenticationCookieContainer.Version}.");
            }

            return new AuthenticationTicket(khumaloCraftPrincipal, authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public byte[] Serialize(AuthenticationTicket model)
        {
            var khumaloCraftPrincipal = (KhumaloCraftPrincipal)model.Principal;

            var impersonator = _httpContextProvider.GetItem(AuthenticationCookieUserData.AuthCookieUserDataKey) as AuthenticationCookieUserData;

            return JsonSerializer.SerializeToUtf8Bytes(AuthenticationCookieContainer.Create(
                new AuthenticationCookieUserData()
                {
                    UserId = khumaloCraftPrincipal.User.Id.Value,
                    UserImpersonationLogId = impersonator?.UserImpersonationLogId,
                    ImpersonatorUserId = impersonator?.ImpersonatorUserId,
                    IssuedUtc = model.Properties.IssuedUtc,
                    ExpiresUtc = model.Properties.ExpiresUtc,
                    IsPersistent = model.Properties.IsPersistent
                }, CurrentCookieVersion));
        }
    }
}
