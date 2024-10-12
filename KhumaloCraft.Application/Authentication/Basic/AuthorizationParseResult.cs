namespace KhumaloCraft.Application.Authentication.Basic;

public enum AuthorizationParseResult
{
    Success = 1,
    Failure = 2,
    MissingCredentials = 3,
    InvalidBasicScheme = 4
}