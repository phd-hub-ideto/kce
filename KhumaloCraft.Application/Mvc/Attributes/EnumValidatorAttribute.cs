using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

/// <summary>
/// A validation attribute for enums.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
public class EnumValidatorAttribute : ValidationAttribute
{
    public EnumValidatorAttribute(Type enumType, bool required = true)
    {
        EnumType = enumType;
        Required = required;
    }

    public Type EnumType { get; private set; }

    public bool Required { get; private set; }

    public override bool IsValid(object value)
    {
        if (!Required && value == null)
        {
            return true;
        }

        if (!Enum.IsDefined(EnumType, value))
        {
            return false;
        }
        return true;
    }
}