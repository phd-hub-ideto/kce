namespace KhumaloCraft.Application.UserContext;

public class LastVisitedLocation
{
    public int LocationId { get; }
    public LastVisitedType Type { get; }

    public LastVisitedLocation(int locationId, LastVisitedType type)
    {
        LocationId = locationId;
        Type = type;
    }
}