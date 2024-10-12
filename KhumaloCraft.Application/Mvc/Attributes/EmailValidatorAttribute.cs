using KhumaloCraft.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace KhumaloCraft.Application.Mvc.Attributes;

/// <summary>
/// Enumerates available email validation error message formats.
/// </summary>
public enum EmailValidatorMessageFormat
{
    /// <summary>
    /// A general plain message.
    /// </summary>
    Plain,

    /// <summary>
    /// A message that references an incorrect field. Suitable for user-interface validation.
    /// </summary>
    ReferenceField,

    /// <summary>
    /// A short plain message, suitable for mobile.
    /// </summary>
    Short,

    /// <summary>
    /// A localized message.
    /// </summary>
    Localized
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
public class EmailValidatorAttribute : DataTypeAttribute//, IClientValidatable
{
    /// <summary>
    /// Constructs an instance of EmailValidatorAttribute.
    /// </summary>
    /// <param name="messageFormat">The format of the validation error message to output.</param>
    /// <param name="allowFullAddressSpecification">
    /// Whether to allow full email address specification, which includes a display name and email address in
    /// angle brackets, e.g. Display-Name &lt;name@domain.com&gt;. Default is False.
    /// </param>
    public EmailValidatorAttribute(EmailValidatorMessageFormat messageFormat = EmailValidatorMessageFormat.Plain, bool allowFullAddressSpecification = false)
        : base(DataType.EmailAddress)
    {
        switch (messageFormat)
        {
            case EmailValidatorMessageFormat.Plain:
                ErrorMessage = "The email address is not in a correct format.";
                break;
            case EmailValidatorMessageFormat.ReferenceField:
                ErrorMessage = "The {0} field is not a valid email address.";
                break;
            case EmailValidatorMessageFormat.Short:
                ErrorMessage = "Please enter a valid email address.";
                break;
            case EmailValidatorMessageFormat.Localized:
                ErrorMessage = "{0} is not valid.";
                break;
        }

        _messageFormat = messageFormat;
        _allowFullAddressSpecification = allowFullAddressSpecification;
    }

    private EmailValidatorMessageFormat _messageFormat;

    /// <summary>
    /// Gets the message format for this validator.
    /// </summary>
    public EmailValidatorMessageFormat MessageFormat
    {
        get
        {
            return _messageFormat;
        }
    }

    private bool _allowFullAddressSpecification;

    /// <summary>
    /// Gets a value indicating whether this validator allows full email address specification, which includes a display name and email address in
    /// angle brackets, e.g. Display-Name &lt;name@domain.com&gt;.
    /// </summary>
    public bool AllowFullAddressSpecification
    {
        get
        {
            return _allowFullAddressSpecification;
        }
    }

    public bool IsValid(string email)
    {
        return string.IsNullOrEmpty(email) || ValidationHelper.IsValidEmailAddress(email, _allowFullAddressSpecification);
    }

    public override bool IsValid(object value)
    {
        return value == null || (value is string && IsValid((string)value));
    }

    #region IClientValidatable Implementation

    private static readonly Regex _validationRegex = new Regex(ValidationHelper.EmailAllowSurroundingWhiteSpaceRegEx,
        RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

    //public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //{
    //    yield return new ModelClientValidationRegexRule(FormatErrorMessage(metadata.GetDisplayName()), _validationRegex.ToString());
    //}

    #endregion

}