using System.Diagnostics;

namespace KhumaloCraft.Application.Monitoring;

public class RequestCountersContext
{
    public RequestCountersContext()
    {
        // to satisfy generic constraint
    }

    //TODO-L : Make accurate counters RequestCountersContext

    public Stopwatch PageTimer { get; set; }

    public string PageDurationText
    {
        get
        {
            if (PageTimer == null)
            {
                return "-";
            }

            return Helpers.FormattingHelper.AsMilliseconds(PageTimer.Elapsed);
        }
    }

    public int SearchEngineCalls { get; set; }

    public TimeSpan SearchEngineDuration { get; set; }

    public string SearchEngineDurationText => Helpers.FormattingHelper.AsMilliseconds(SearchEngineDuration);

    public int SqlCalls { get; set; }

    public TimeSpan SqlDuration { get; set; }

    public string SqlDurationText => Helpers.FormattingHelper.AsMilliseconds(SqlDuration);

    public int ReactCalls { get; set; }

    public TimeSpan ReactDuration { get; set; }

    public string ReactDurationText => Helpers.FormattingHelper.AsMilliseconds(ReactDuration);

    public IEnumerable<KeyValuePair<string, int>> RawValues()
    {
        if (PageTimer != null)
        {
            yield return new KeyValuePair<string, int>("Page-Time", (int)PageTimer.ElapsedMilliseconds);
            yield return new KeyValuePair<string, int>("Sql-Time", (int)SqlDuration.TotalMilliseconds);
            yield return new KeyValuePair<string, int>("Sql-Count", SqlCalls);
            yield return new KeyValuePair<string, int>("Search-Time", (int)SearchEngineDuration.TotalMilliseconds);
            yield return new KeyValuePair<string, int>("Search-Count", SearchEngineCalls);
            yield return new KeyValuePair<string, int>("React-Time", (int)ReactDuration.TotalMilliseconds);
            yield return new KeyValuePair<string, int>("React-Count", ReactCalls);
        }
    }
}