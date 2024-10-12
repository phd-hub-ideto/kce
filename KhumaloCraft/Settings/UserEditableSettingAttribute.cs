namespace KhumaloCraft;

[AttributeUsage(AttributeTargets.Property)]
public class UserEditableSettingAttribute : Attribute
{
    public string Description { get; }

    public UserEditableSettingAttribute() { }

    public UserEditableSettingAttribute(string description)
    {
        Description = description;
    }
}
