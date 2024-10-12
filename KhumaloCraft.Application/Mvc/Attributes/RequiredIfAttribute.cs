using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class RequiredIfAttribute : ConditionalAttributeBase
{
    private ValidationAttribute _innerAttribute;

    public string DependentPropertyName { get; set; }
    public object[] TargetValues { get; set; }

    public RequiredIfAttribute(string dependentProperty, params object[] targetValues)
        : this(null, dependentProperty, null, targetValues)
    {
    }

    public RequiredIfAttribute(Type validationAttributeType, string dependentPropertyName, params object[] targetValues)
        : this(validationAttributeType, dependentPropertyName, null, targetValues)
    {
    }

    public RequiredIfAttribute(Type validationAttributeType, string dependentPropertyName, string errorMessage, params object[] targetValues)
        : base(errorMessage)
    {
        if (validationAttributeType?.IsSubclassOf(typeof(ValidationAttribute)) == false)
        {
            throw new ArgumentException("Specified type must derive from ValidationAttribute", nameof(validationAttributeType));
        }

        DependentPropertyName = dependentPropertyName;
        TargetValues = targetValues;

        if (validationAttributeType != null)
        {
            _innerAttribute = (ValidationAttribute)Activator.CreateInstance(validationAttributeType);
        }
        else
        {
            _innerAttribute = new RequiredAttribute();
        }
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        //If value implements IRequiredIfValueProvider, then we need to get the mapped value to check for validity.
        IRequiredIfValueProvider valueProvider = value as IRequiredIfValueProvider;
        if (valueProvider != null)
        {
            value = valueProvider.GetValue();
        }

        var shouldRunValidation = false;

        foreach (var targetValue in TargetValues)
        {
            if (ShouldRunValidation(value, DependentPropertyName, targetValue, validationContext))
            {
                shouldRunValidation = true;
                break;
            }
        }

        if (shouldRunValidation && !_innerAttribute.IsValid(value))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
        }

        return ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
    {
        if (!string.IsNullOrEmpty(ErrorMessageString))
            _innerAttribute.ErrorMessage = ErrorMessageString;

        return _innerAttribute.FormatErrorMessage(name);
    }
}
