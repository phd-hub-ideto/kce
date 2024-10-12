namespace KhumaloCraft.Dependencies;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class RegistrationAttribute : Attribute
{
    protected RegistrationAttribute(LifestyleType lifestyle)
    {
        Lifestyle = lifestyle;
    }

    public LifestyleType Lifestyle { get; }

    public Type Contract { get; set; }
}