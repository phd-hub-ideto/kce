using KhumaloCraft.Domain.Team;

namespace KhumaloCraft.Application.Portal.Models.Home;

public sealed class TeamMemberModel
{
    public int Id { get; internal set; }
    public string Name { get; internal set; }
    public Role Role { get; internal set; }
    public string EmailAddress { get; internal set; }
    public string ImageUrl { get; internal set; }
}
