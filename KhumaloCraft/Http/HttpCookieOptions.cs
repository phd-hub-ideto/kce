namespace KhumaloCraft.Http;

public class HttpCookieOptions
{
    public DateTime? ExpiryDate { get; }
    public bool HasExpiryDate => ExpiryDate.HasValue;
    public string Domain { get; }
    public bool Secure { get; }
    public SameSiteMode SameSiteMode { get; }

    public HttpCookieOptions(
        DateTime? expiryDate = null,
        string domain = null,
        bool secure = true,
        SameSiteMode sameSiteMode = SameSiteMode.Lax)
    {
        ExpiryDate = expiryDate;
        Domain = domain;
        Secure = secure;
        SameSiteMode = sameSiteMode;
    }

    public HttpCookieOptions(
        TimeSpan expiryTime,
        string domain = null,
        bool secure = true,
        SameSiteMode sameSiteMode = SameSiteMode.Lax)
    {
        ExpiryDate = DateTime.Now.Add(expiryTime);
        Domain = domain;
        Secure = secure;
        SameSiteMode = sameSiteMode;
    }
}