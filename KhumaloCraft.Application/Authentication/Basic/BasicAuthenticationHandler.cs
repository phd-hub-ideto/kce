using KhumaloCraft.Domain.Authentication.Passwords;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;

namespace KhumaloCraft.Application.Authentication.Basic;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "basic";

    private readonly IPasswordValidator _principalService;
    private readonly IBasicAuthenticationHelper _basicAuthenticationHelper;
    private readonly IUserRolePermissionRepository _userRolePermissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IPasswordValidator principalService,
        IBasicAuthenticationHelper basicAuthenticationHelper,
        IUserRolePermissionRepository userRolePermissionRepository,
        IUserRoleRepository userRoleRepository) : base(options, logger, encoder, clock)
    {
        _principalService = principalService;
        _basicAuthenticationHelper = basicAuthenticationHelper;
        _userRolePermissionRepository = userRolePermissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers[HeaderNames.Authorization].FirstOrDefault();
        if (authorizationHeader == null) return Task.FromResult(AuthenticateResult.Fail("Authorization header not present."));

        var authenticationHeaderValue = AuthenticationHeaderValue.Parse(authorizationHeader);

        var parseResult = _basicAuthenticationHelper.TryParse(authenticationHeaderValue, out string username, out string password);

        switch (parseResult)
        {
            case AuthorizationParseResult.Success:
                return Task.FromResult(ValidateAndAssignPrincipal(username, password));
            case AuthorizationParseResult.InvalidBasicScheme:
                return Task.FromResult(AuthenticateResult.Fail("Invalid scheme"));
            case AuthorizationParseResult.MissingCredentials:
                return Task.FromResult(AuthenticateResult.Fail("Missing credentials"));
            default:
                return Task.FromResult(AuthenticateResult.Fail(""));
        }
    }

    public AuthenticateResult ValidateAndAssignPrincipal(string username, string password)
    {
        if (_principalService.TryValidatePassword(username, password, false, out var user, out _))
        {
            var khumaloCraftPrincipal = KhumaloCraftPrincipal.Create(user, _userRolePermissionRepository, _userRoleRepository);

            return AuthenticateResult.Success(new AuthenticationTicket(khumaloCraftPrincipal, AuthenticationScheme));
        }
        return AuthenticateResult.Fail("");
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = "Basic";

        return base.HandleChallengeAsync(properties);
    }
}