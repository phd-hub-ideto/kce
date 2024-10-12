using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class EmailAddressFieldValidator : ValidationAttribute
{
    private readonly EmailValidatorMessageFormat _messageFormat;
    private readonly bool _allowFullAddressSpecification;

    // Default
    public EmailAddressFieldValidator()
    {
        _messageFormat = EmailValidatorMessageFormat.ReferenceField;
    }

    // For Localized
    public EmailAddressFieldValidator(EmailValidatorMessageFormat messageFormat = EmailValidatorMessageFormat.Plain, bool allowFullAddressSpecification = false)
    {
        _messageFormat = messageFormat;
        _allowFullAddressSpecification = allowFullAddressSpecification;
    }

    public override bool IsValid(object value)
    {
        var emailAddressValidator = new EmailValidatorAttribute(_messageFormat, _allowFullAddressSpecification);

        var emailAddressValid = emailAddressValidator.IsValid(value);

        if (!emailAddressValid)
        {
            ErrorMessage = emailAddressValidator.ErrorMessage;

            return false;
        }

        var domainValidator = new EmailDomainCheckerAttribute();

        var domainValid = domainValidator.IsValid(value);

        if (!domainValid)
        {
            ErrorMessage = domainValidator.ErrorMessage;

            return false;
        }

        return true;
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class LocalizedEmailAddressValidator : EmailAddressFieldValidator
{
    public LocalizedEmailAddressValidator(bool allowFullAddressSpecification = false)
        : base(EmailValidatorMessageFormat.Localized, allowFullAddressSpecification)
    {

    }
}