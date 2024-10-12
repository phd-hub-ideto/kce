using KhumaloCraft.Helpers;
using System.ComponentModel.DataAnnotations;

namespace KhumaloCraft.Application.Mvc.Attributes;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public class EmailDomainCheckerAttribute : ValidationAttribute
{
    public EmailDomainCheckerAttribute()
    {
        ErrorMessage = "The email domain specified is not valid for delivering email, possibly missing MX record. Please check spelling.";
    }

    public override bool IsValid(object value)
    {
        var val = value as string;

        var settings = Dependencies.DependencyManager.Container.GetInstance<ISettings>();

        return string.IsNullOrEmpty(val) || ValidationHelper.HasValidMailLookup(val, settings.EnableEmailDomainChecking);
    }
}