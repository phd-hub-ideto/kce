using System.Text.Json.Serialization;

namespace KhumaloCraft.Application.Cookies;

public class AuthenticationCookieUserData
{
    [JsonIgnore]
    public static object AuthCookieUserDataKey = new object();

    [JsonPropertyName("uid")]
    public int UserId { get; set; }

    [JsonPropertyName("iui")]
    public int? ImpersonatorUserId { get; set; }

    [JsonPropertyName("uili")]
    public int? UserImpersonationLogId { get; set; }

    [JsonPropertyName("ucidu")]
    public DateTimeOffset? IssuedUtc { get; set; }

    [JsonPropertyName("ucedu")]
    public DateTimeOffset? ExpiresUtc { get; set; }

    [JsonPropertyName("ucip")]
    public bool IsPersistent { get; set; }
}