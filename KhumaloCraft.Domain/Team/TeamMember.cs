namespace KhumaloCraft.Domain.Team;

public sealed class TeamMember
{
    public int Id { get; internal set; }
    public string Name { get; internal set; }
    public Role Role { get; internal set; }
    public string EmailAddress { get; internal set; }
    public string ImageUrl { get; internal set; }
}
