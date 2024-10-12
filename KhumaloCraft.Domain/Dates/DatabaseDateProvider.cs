using KhumaloCraft.Data.Entities;
using KhumaloCraft.Dependencies;

namespace KhumaloCraft.Domain.Dates;

[Singleton]
public class DatabaseDateProvider : IDatabaseDateProvider
{
    public DateTime GetDate()
    {
        using (var scope = DalScope.Begin())
        {
            //TODO-LP : Fix server date
            //return scope.KhumaloCraft.ServerDate();

            return DateTime.Now;
        }
    }
}