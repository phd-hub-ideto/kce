namespace KhumaloCraft.Domain.Security.Role;

[Serializable]
public class Role : IIdentifier
{
    public int? Id { get; set; }
    public int? SecurityEntityTypeId { get; set; }
    public SecurityEntityType? SecurityEntityType
    {
        get { return (SecurityEntityType?)SecurityEntityTypeId; }
        set { SecurityEntityTypeId = (int?)value; }
    }
    public string Name { get; set; }

    public List<PermissionIdentifier> Permissions { get; set; } = new List<PermissionIdentifier>();

    public bool IsNew => null == Id || null == SecurityEntityTypeId;

    public Role() { }

    int IIdentifier.Id
    {
        get
        {
            if (null == Id)
                throw new InvalidOperationException("Role has no Id as it has not yet been committed to the database");
            return Id.Value;
        }
    }

    string IIdentifier.Name
    {
        get { return Name; }
    }

    public override string ToString()
    {
        return Name;
    }
}
