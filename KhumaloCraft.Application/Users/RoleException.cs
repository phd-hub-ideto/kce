namespace KhumaloCraft.Application.Users;

[Serializable]
public class RoleException : Exception
{
    public RoleException() { }
    public RoleException(string message) : base(message) { }
    public RoleException(string message, Exception inner) : base(message, inner) { }
}