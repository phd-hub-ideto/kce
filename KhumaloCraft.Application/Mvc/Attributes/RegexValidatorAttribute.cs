using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace KhumaloCraft.Application.Mvc.Attributes;

/// <summary>
/// A validation attribute for strings
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
public class RegexValidatorAttribute : ValidationAttribute
{
    public RegexValidatorAttribute(string pattern)
    {
        _regex = new Regex(pattern);
    }

    private readonly Regex _regex;

    public override bool IsValid(object value)
    {

        return value == null || _regex.IsMatch(value.ToString());
    }
}