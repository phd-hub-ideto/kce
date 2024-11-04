namespace KhumaloCraft.Domain.SiteVisits;

public class SiteVisitLog
{
    public string RequestPath { get; set; }
    public string ContentType { get; set; }
    public string Scheme { get; set; }
    public string Referrer { get; set; }
    public string Method { get; set; }
    public string Host { get; set; }
    public int StatusCode { get; set; }
    public string Location { get; set; }
    public string UserAgent { get; set; }
    public string Platform { get; set; }
    public string IpAddress { get; set; }
    public int? UserId { get; set; }
    public string UniqueUserId { get; set; }
}