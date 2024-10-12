namespace KhumaloCraft.Dependencies;

public class SingletonAttribute : RegistrationAttribute
{
    public SingletonAttribute() : base(LifestyleType.Singleton)
    {

    }
}