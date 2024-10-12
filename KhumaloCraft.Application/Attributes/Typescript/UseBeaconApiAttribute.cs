namespace KhumaloCraft.Application.Attributes.TypeScript;

/// <summary>
/// Used to indicate use the Beacon Api instead of AJAX when generating the client side code.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class UseBeaconApiAttribute : Attribute
{

}