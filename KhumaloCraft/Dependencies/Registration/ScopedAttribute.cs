namespace KhumaloCraft.Dependencies;

public class ScopedAttribute : RegistrationAttribute
{
    public ScopedAttribute() : base(LifestyleType.Scoped)
    {

    }
}