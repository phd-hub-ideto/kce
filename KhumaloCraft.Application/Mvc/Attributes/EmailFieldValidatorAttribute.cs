namespace KhumaloCraft.Application.Mvc.Attributes;

public class EmailFieldValidatorAttribute : EmailValidatorAttribute
{
    /// <summary>
    /// Constructs an instance of EmailFieldValidatorAttribute with a field-referenced message, and that disallowed full email address specifications.
    /// </summary>
    public EmailFieldValidatorAttribute()
        : base(EmailValidatorMessageFormat.ReferenceField)
    {

    }
}