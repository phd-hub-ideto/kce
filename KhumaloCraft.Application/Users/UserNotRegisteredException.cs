namespace KhumaloCraft.Application.Users;

[Serializable]
public class UserNotRegisteredException : Exception
{
    public UserNotRegisteredException()
        : base()
    { }

    public UserNotRegisteredException(string message)
        : base(message)
    { }

    public UserNotRegisteredException(string message, Exception inner)
        : base(message, inner)
    { }
}