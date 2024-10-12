using KhumaloCraft.Dependencies;
using KhumaloCraft.Helpers;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Validation.Email;

[Singleton(Contract = typeof(EmailAddressValidator))]
public class EmailAddressValidator
{
    private readonly ISettings _settings;

    public EmailAddressValidator(ISettings settings)
    {
        _settings = settings;
    }

    public IEnumerable<ValidationResult> Validate(string emailAddress, string memberName)
    {
        var validEmail = ValidationHelper.IsValidEmailAddress(emailAddress, false);

        if (!validEmail)
        {
            yield return new ValidationResult("The email address is not in a correct format.", [memberName]);
        }

        if (validEmail && !ValidationHelper.HasValidMailLookup(emailAddress, _settings.EnableEmailDomainChecking))
        {
            yield return new ValidationResult("The email address is not valid for delivering email, possibly missing MX record. Please check spelling.", [memberName]);
        }
    }
}