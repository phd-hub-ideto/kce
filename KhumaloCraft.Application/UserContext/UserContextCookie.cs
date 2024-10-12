using Newtonsoft.Json;

namespace KhumaloCraft.Application.UserContext;

public class UserContextCookie
{
    public UserContextCookieUserDetails User { get; set; }

    public int? CityId { get; set; }
    public int? SuburbId { get; set; }
    public int? ProvinceId { get; set; }
    public bool? HideMapSearch { get; set; }
    public bool? ShowCountryDetectionDialog { get; set; }
    public RecentSearchBar RecentSearches { get; set; }
}

public class UserContextCookieUserDetails
{
    public string FullName { get; set; }
    public string MobileNumber { get; set; }
    public string EmailAddress { get; set; }
}

public class UserLegacyContextCookieUserDetails
{
    [JsonProperty("Name")]
    public string FullName { get; set; }

    [JsonProperty("Number")]
    public string MobileNumber { get; set; }

    [JsonProperty("Email")]
    public string EmailAddress { get; set; }
}