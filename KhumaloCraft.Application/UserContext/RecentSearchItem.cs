using KhumaloCraft.Domain.Enumerations;

namespace KhumaloCraft.Application.UserContext;

public class RecentSearchItem
{
    public AreaType AreaType { get; set; }
    public int AreaId { get; set; }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var comparedTo = (RecentSearchItem)obj;

        return AreaType == comparedTo.AreaType && AreaId == comparedTo.AreaId;
    }

    public override int GetHashCode()
    {
        return AreaType.GetHashCode() ^ AreaId.GetHashCode();
    }

    public RecentSearchItem(int areaId, AreaType areaType)
    {
        AreaId = areaId;
        AreaType = areaType;
    }
}
