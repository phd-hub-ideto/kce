using Newtonsoft.Json;

namespace KhumaloCraft.Application.Users.UserPreferences;

public class UserPreferencesCookie
{
    [JsonProperty("ShowDebugDfpAds")]
    public bool ShowDebugGoogleAds { get; set; }
}