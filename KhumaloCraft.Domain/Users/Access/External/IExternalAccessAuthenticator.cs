namespace KhumaloCraft.Domain.Users.Access.External;

public interface IExternalAccessAuthenticator
{
    void Login(ExternalUserLoginParameters loginParameters);

    User ServiceLogin(ExternalUserLoginParameters loginParameters);
}