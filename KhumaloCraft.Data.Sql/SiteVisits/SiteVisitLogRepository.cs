using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.SiteVisits;

namespace KhumaloCraft.Data.Sql.SiteVisits;

[Singleton(Contract = typeof(ISiteVisitLogRepository))]
public sealed class SiteVisitLogRepository : ISiteVisitLogRepository
{
    public void Insert(SiteVisitLog siteVisitLog)
    {
        using var scope = DalScope.Begin();

        var dalSiteVisitLog = new DalSiteVisitLog
        {
            DateVisited = scope.ServerDate(),
            RequestPath = siteVisitLog.RequestPath,
            ContentType = siteVisitLog.ContentType,
            Scheme = siteVisitLog.Scheme,
            Referrer = siteVisitLog.Referrer,
            Method = siteVisitLog.Method,
            Host = siteVisitLog.Host,
            StatusCode = siteVisitLog.StatusCode,
            Location = siteVisitLog.Location,
            UserAgent = siteVisitLog.UserAgent,
            Platform = siteVisitLog.Platform,
            IpAddress = siteVisitLog.IpAddress,
            UserId = siteVisitLog.UserId,
            UniqueUserId = siteVisitLog.UniqueUserId,
        };

        scope.KhumaloCraft.SiteVisitLog.Add(dalSiteVisitLog);

        scope.Commit();
    }
}