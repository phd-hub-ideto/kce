namespace KhumaloCraft.Application.Session;

public static class LegacyCookieHelper
{
    public static IDictionary<string, string> FromLegacyCookieString(string legacyCookie)
    {
        var parsed = new Dictionary<string, string>();

        var parts = legacyCookie.Split('&');
        foreach (var part in parts)
        {
            var kvp = part.Split('=');
            if (kvp.Length == 2)
            {
                parsed[kvp[0]] = kvp[1];
            }
        }

        return parsed;
    }

    public static string ToLegacyCookieString(IDictionary<string, string> dict)
    {
        return string.Join("&", dict.Select(kvp => string.Join("=", kvp.Key, kvp.Value)));
    }
}