namespace KhumaloCraft.Domain.Users.Access;

public class VerifyAccountResult
{
    public bool Success { get; set; }
    public VerifyAccountStatus State { get; set; }
    public string Message { get; set; }
    public string ReturnUrl { get; set; }
}