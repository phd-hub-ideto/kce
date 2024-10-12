using KhumaloCraft.Dependencies;

namespace KhumaloCraft.Domain.Dates;

[Singleton]
public class DateProvider : IDateProvider
{
    public DateTime GetDate()
    {
        return DateTime.Now;
    }
}