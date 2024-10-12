namespace KhumaloCraft.Domain.Team;

public sealed class TeamService : ITeamService
{
    private static readonly List<TeamMember> _teamMembers =
    [
        new TeamMember
        {
            Id = 1,
            Name = "James Khumalo",
            Role = Role.FounderAndCeo,
            EmailAddress = "james@khumalocraft.co.za",
            ImageUrl = "/images/team/james-khumalo.jpeg"
        },
        new TeamMember
        {
            Id = 2,
            Name = "McKaylar Rostova",
            Role = Role.ChiefOperatingOfficer,
            EmailAddress = "mckaylar.rostova@khumalocraft.co.za",
            ImageUrl = "/images/team/coo.jpeg"
        },
        new TeamMember
        {
            Id = 3,
            Name = "Aram Mojtabai",
            Role = Role.ChiefTechnicalOfficer,
            EmailAddress = "aram.mojtabai@khumalocraft.co.za",
            ImageUrl = "/images/team/cto.jpeg"
        },
        new TeamMember
        {
            Id = 4,
            Name = "Elizabeth Keen",
            Role = Role.HeadOfHr,
            EmailAddress = "elizabeth.keen@khumalocraft.co.za",
            ImageUrl = "/images/team/head-of-hr.jpeg"
        },
        new TeamMember
        {
            Id = 5,
            Name = "Jessica Nhlapo",
            Role = Role.MarketingDirector,
            EmailAddress = "jessica.nhlapo@khumalocraft.co.za",
            ImageUrl = "/images/team/marketing-director.jpeg"
        }
    ];

    public IEnumerable<TeamMember> GetAllTeamMembers()
    {
        return _teamMembers;
    }

    public TeamMember GetTeamMemberById(int id)
    {
        return _teamMembers.Single(c => c.Id == id);
    }

    public bool TryGetTeamMemberById(int id, out TeamMember teamMember)
    {
        teamMember = _teamMembers.SingleOrDefault(c => c.Id == id);

        return teamMember != null;
    }
}
