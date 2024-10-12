namespace KhumaloCraft.Domain.Users.Access;

public class LoginParameters
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool LogUserLogin { get; set; }
    public bool PersistCookie { get; set; }
    public LoginSource LoginSource { get; set; }
}

public enum LoginSource
{
    Manage,
    Portal,
    Mobile
}