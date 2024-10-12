using KhumaloCraft.Domain.Validation;

namespace KhumaloCraft.Domain.Security.Role;

public interface IRoleRepository : IValidatingRepository<Role>
{
    void Upsert(Role role);
    IQueryable<Role> Query();
    Role Fetch(int id);
    Role FetchByName(string name);
    bool TryFetchByName(string name, out Role role);
}