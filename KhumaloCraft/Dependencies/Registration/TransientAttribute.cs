namespace KhumaloCraft.Dependencies;

public class TransientAttribute : RegistrationAttribute
{
    public TransientAttribute() : base(LifestyleType.Transient)
    {

    }
}
