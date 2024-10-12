namespace KhumaloCraft.Domain.Team;

public interface ITeamService
{
    IEnumerable<TeamMember> GetAllTeamMembers();
    TeamMember GetTeamMemberById(int id);
    public bool TryGetTeamMemberById(int id, out TeamMember teamMember);
}