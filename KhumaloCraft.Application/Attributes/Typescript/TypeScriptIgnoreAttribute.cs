namespace KhumaloCraft.Application.Attributes.TypeScript;

/// <summary>
/// Used to indicate when Type Script properties should be ignored
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Field)]
public class TypeScriptIgnoreAttribute : Attribute
{

}