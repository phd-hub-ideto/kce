namespace KhumaloCraft.Application.Users;

public class RegistrationResult
{
    // only allow construction via factory methods
    private RegistrationResult()
    { }

    public bool IsSuccess => Status == RegistrationStatus.Success;

    public RegistrationStatus Status { get; private set; }
    public string Message { get; private set; }
    public int UserId { get; private set; }

    public void ThrowIfUnsuccessful()
    {
        if (IsSuccess == false)
        {
            throw new Exception(Message);
        }
    }

    public static RegistrationResult Success(int userId)
    {
        return new RegistrationResult { Status = RegistrationStatus.Success, UserId = userId };
    }

    public static RegistrationResult Failure(RegistrationStatus failureStatus, string message = null)
    {
        return new RegistrationResult { Status = failureStatus, Message = message, };
    }
}

public enum RegistrationStatus
{
    Success,
    InvalidFirstNameOrLastName,
    InvalidPassword,
    UserAlreadyExists,
}