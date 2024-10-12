namespace KhumaloCraft.Application.UserContext;

public class RecentSearchBar
{
    public List<RecentSearchItem> RecentSearchItems { get; private set; }

    public RecentSearchBar()
    {
        RecentSearchItems = new List<RecentSearchItem>();
    }

    public void AddRecentSearch(RecentSearchItem recentSearchItem)
    {
        if (recentSearchItem != null)
        {
            var latestList = new List<RecentSearchItem>
            {
                recentSearchItem,
            };

            latestList.AddRange(RecentSearchItems.Where(s => !s.Equals(recentSearchItem)).Take(Constants.RecentSearchMaxDisplay - 1));

            RecentSearchItems = latestList;
        }
    }
}
