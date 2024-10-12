namespace KhumaloCraft.Application.Mvc.Attributes;

/// <summary>
/// A validation attribute for hex color strings
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
public class HexColourValidatorAttribute : RegexValidatorAttribute
{
    public HexColourValidatorAttribute() : base("^#([A-Fa-f0-9]{6}(?:[A-Fa-f0-9]{2})?|[A-Fa-f0-9]{3}(?:[A-Fa-f0-9]{1})?)$")
    { }
}
