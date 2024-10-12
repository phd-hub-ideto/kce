using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

public class ConditionalAttributeBase : ValidationAttribute
{
    private const string DefaultErrorMessage = "The {0} field was invalid";

    public ConditionalAttributeBase()
        : this(DefaultErrorMessage)
    {
    }

    public ConditionalAttributeBase(string errorMessage)
        : base(errorMessage)
    {
    }

    protected bool ShouldRunValidation(object value, string dependentPropertyName, object targetValue, ValidationContext validationContext)
    {
        var dependentValue = GetDependentFieldValue(dependentPropertyName, validationContext);

        // compare the value against the target value
        if (dependentValue == null && targetValue == null) return true;

        if (dependentValue != null && targetValue != null)
        {
            if (targetValue.GetType().IsArray)
                return ((object[])targetValue).Any(i => i.Equals(dependentValue));
            else
                return dependentValue.Equals(targetValue);
        }

        return false;
    }

    // TODO-L: Some of these could be removed from the base class. They also need dedicated unit tests.
    public static object GetDependentFieldValue(string dependentProperty, ValidationContext validationContext)
    {
        // get a reference to the property this validation depends upon
        var containerType = validationContext.ObjectInstance.GetType();
        var field = containerType.GetProperty(dependentProperty);

        if (field == null)
            throw new MissingMemberException(containerType.Name, dependentProperty);

        // get the value of the dependent property
        var dependentvalue = field.GetValue(validationContext.ObjectInstance, null);

        return dependentvalue;
    }
}