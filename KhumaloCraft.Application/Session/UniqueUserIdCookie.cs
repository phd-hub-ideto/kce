namespace KhumaloCraft.Application.Session;

public class UniqueUserIdCookie
{
    public const string IdKey = "Id";
    public const string DateKey = "Date";

    public UniqueUserIdCookie(string id, DateTime date)
    {
        Id = id;
        Date = date;
    }

    public string Id { get; }
    public DateTime Date { get; }

    public static bool TryParse(string cookie, out UniqueUserIdCookie uniqueUserIdCookie)
    {
        var cookieParts = LegacyCookieHelper.FromLegacyCookieString(cookie);

        if (!cookieParts.TryGetValue(DateKey, out var rawDateValue) || !ParseDate(rawDateValue, out var date))
        {
            uniqueUserIdCookie = null;
            return false;
        }

        if (!cookieParts.TryGetValue(IdKey, out var id) || id.Length != 24)
        {
            uniqueUserIdCookie = null;
            return false;
        }

        uniqueUserIdCookie = new UniqueUserIdCookie(id, date);
        return true;
    }

    private static bool ParseDate(string rawDateValue, out DateTime date)
    {
        if (long.TryParse(rawDateValue, out var longDateValue))
        {
            date = new DateTime(longDateValue);
            return true;
        }

        return DateTime.TryParse(rawDateValue, out date);
    }

    public override string ToString()
    {
        return LegacyCookieHelper.ToLegacyCookieString(new Dictionary<string, string>()
        {
            { IdKey, Id },
            { DateKey, Date.Ticks.ToString() }
        });
    }
}
