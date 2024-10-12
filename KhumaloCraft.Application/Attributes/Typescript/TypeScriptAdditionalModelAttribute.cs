namespace KhumaloCraft.Application.Attributes.TypeScript;

/// <summary>
/// Used to indicate when TypeScript Models should be generated for a class
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface)]
public class TypeScriptAdditionalModelAttribute : Attribute
{
    /// <summary>
    /// Should the generator extend from the base interface or flatten the interface.
    /// If <see cref="true"/> then the interface will extend the base interface.
    /// </summary>
    public bool InheritFromBase { get; }

    /// <summary>
    /// Default constructor; interface will be flattened.
    /// </summary>
    public TypeScriptAdditionalModelAttribute()
    {
        InheritFromBase = false;
    }

    /// <summary>
    /// Overloaded constructor specifying whether the interface should inherit or not.
    /// </summary>
    /// <param name="inheritFromBase">Should interface inherit from the base.</param>
    public TypeScriptAdditionalModelAttribute(bool inheritFromBase = false)
    {
        InheritFromBase = inheritFromBase;
    }
}