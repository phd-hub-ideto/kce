using KhumaloCraft.Dependencies;

namespace KhumaloCraft.Domain.Dates;

[Singleton]
public class DateProvider : IDateProvider
{
    public DateTime GetDate()
    {
        //TODO-LP : Better way for determining time. For now, we will use South African Time (UTC + 2 hours)
        return DateTime.UtcNow.AddHours(2);
    }
}