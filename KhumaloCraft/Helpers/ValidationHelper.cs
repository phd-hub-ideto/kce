using System.Net.Mail;
using System.Text.RegularExpressions;

namespace KhumaloCraft.Helpers;

public static class ValidationHelper
{
    public const string UrlRegexRelaxed = @"(^(http|ftp|https):\/\/)?[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#!]*[\w\-\@?^=%&amp;/~\+#])?";

    public const string UrlRegEx = @"^(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#!]*[\w\-\@?^=%&amp;/~\+#])?";

    public const string HostNameSearchRegEx = @"(?:[a-zA-Z0-9-]+\.){2,}[a-zA-Z]{2,}";
    private const string DomainRegExInner = @"(?:(?!-)[a-zA-Z0-9-]*[a-zA-Z0-9]\.)+[a-zA-Z]{2,}";

    public const string DomainRegEx = @"^" + DomainRegExInner + "$";

    public const string EmailSearchRegex = @"[\w]+@[\w]+(\.[\w]{2,3})+";
    private const string EmailRegExInner = @"(?![.-])(?:[-a-zA-Z0-9!#$%&*+/=?^_`{|}~.])*(?:[a-zA-Z0-9!#$%&*+/=?^_`{|}~])+@" + DomainRegExInner;

    public const string EmailRegEx = @"^" + EmailRegExInner + "$";

    public const string EmailAllowSurroundingWhiteSpaceRegEx = @"^(\s*)" + EmailRegExInner + @"(\s*)$";

    public const string NameRegEx = @"^[a-zA-ZàáâäãåèéêëìíîïòóôöõøùúûüÿýñçčšžÀÁÂÄÃÅÈÉÊËÌÍÎÏÒÓÔÖÕØÙÚÛÜŸÝÑßÇŒÆČŠŽ∂ð.',0-9\s\-]+$";

    public const string IllegalNameCharsRegex = @"[\t\n\r\/\\:;_@#$%^*\(\)+=\{\}\[\]<>|~]+";

    public const string AgentNameRegEx = @"^[a-zA-ZàáâäãåèéêëìíîïòóôöõøùúûüÿýñçčšžÀÁÂÄÃÅÈÉÊËÌÍÎÏÒÓÔÖÕØÙÚÛÜŸÝÑßÇŒÆČŠŽ∂ð0-9,.'-/\s&]+$";

    public const string NonWordCharsRegEx = @"[\W_]+"; // Matches non-word characters (including white space)

    public const string InvalidEmailSubjectCharsRegEx = @"[\t\n\r;/\\\'\""_]+";

    public const string WhiteSpaceRegex = @"[\s]+";

    public const string VersionNumberRegex = @"^(\d+\.){3}(\d+)$";

    public const string SingleSpace = " ";

    private static readonly char[] _angleBrackets = ['<', '>'];

    public static bool IsValidUri(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return false;
        }

        if (!Regex.IsMatch(uri, @"^(http|ftp|https):\/\/", RegexOptions.IgnoreCase))
        {
            uri = $"http://{uri}";
        }

        return Regex.IsMatch(uri, UrlRegEx, RegexOptions.IgnoreCase);
    }

    public static bool CorrectMissingUrlSchemeAndValidate(string url, out string newUrl)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            newUrl = null;
            return false;
        }

        if (!url.StartsWith("http"))
        {
            newUrl = $"http://{url}";
        }
        else
        {
            newUrl = url;
        }

        var result = Regex.IsMatch(newUrl, UrlRegEx, RegexOptions.IgnoreCase);

        if (!result)
        {
            newUrl = null;
        }
        return result;
    }

    public static bool IsValidUriStrict(string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
            return false;

        return Regex.IsMatch(uri, UrlRegEx);
    }

    public static bool IsValidDomain(string domain)
    {
        return Regex.IsMatch(domain, DomainRegEx);
    }

    /// <summary>
    /// Determines if the given email address is valid, and reachable,
    /// </summary>
    /// <param name="emailAddress"></param>
    public static bool IsValidAndReachableEmailAddress(string emailAddress, ISettings settings)
    {
        return IsValidEmailAddress(emailAddress, true) && HasValidMailLookup(emailAddress, settings.EnableEmailDomainChecking);
    }

    /// <summary>
    /// Determines if the given email address is valid, allowing for any leading or trailing whitespace,
    /// and allowing for full email address specification, which includes a display name and email address in
    /// angle brackets, e.g. Display-Name &lt;name@domain.com&gt;
    /// </summary>
    /// <param name="emailAddress"></param>
    public static bool IsValidEmailAddress(string emailAddress)
    {
        return IsValidEmailAddress(emailAddress, true);
    }

    /// <summary>
    /// Determines if the given email address is valid, allowing for any leading or trailing whitespace,
    /// and based on whether to allow full email address specification, which includes a display name and email address in
    /// angle brackets, e.g. Display-Name &lt;name@domain.com&gt;
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <param name="allowFullAddressSpecification"></param>
    public static bool IsValidEmailAddress(string emailAddress, bool allowFullAddressSpecification)
    {
        if (string.IsNullOrWhiteSpace(emailAddress) ||
            (!allowFullAddressSpecification && emailAddress.IndexOfAny(_angleBrackets) > -1) ||
            emailAddress.Contains("..", StringComparison.Ordinal))
        {
            return false;
        }

        var email = emailAddress.Trim();

        var spaceIndex = email.IndexOf(' ');

        if (spaceIndex > -1)
        {
            var angleBracketIndexes = GetEnclosingAngleBracketIndexes(email, out bool isEnclosed);

            var hasInvalidSpace = !isEnclosed || email.IndexOf(' ', angleBracketIndexes.Item1) > -1;
            if (hasInvalidSpace)
            {
                return false;
            }
        }

        try
        {
            var mailAddress = new MailAddress(email);

            //Max length is 254, according to rfc
            //https://www.rfc-editor.org/errata_search.php?eid=1690

            return mailAddress.Address.Length <= 254 && Regex.IsMatch(mailAddress.Address, EmailRegEx, RegexOptions.IgnoreCase);
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Determines if the given email address has a valid domain via DNS check
    /// </summary>
    /// <param name="value"></param>
    /// <param name="checkDomainSettingEnabled"></param>
    /// <returns></returns>
    public static bool HasValidMailLookup(string value, bool checkDomainSettingEnabled)
    {
        if (!checkDomainSettingEnabled)
        {
            return true;
        }

        try
        {
            /* TODO-LP: Create a DNS Resolver to do mail lookup
            var domain = value.IndexOf('@') >= 0 ? value.Split('@')[1] : value;
            var lookup = DnsResolver.MailLookup(domain);
            var result = lookup.Result;
            return result.Length > 0;
            */
        }
        catch (Exception)
        {
            // Ignore
        }

        return false;
    }

    public static Tuple<int, int> GetEnclosingAngleBracketIndexes(string email, out bool isEnclosed)
    {
        const int notFoundIndex = -1;

        var hasClosingAngleBracket = email != null && email.EndsWith(">", StringComparison.Ordinal);
        if (!hasClosingAngleBracket)
        {
            isEnclosed = false;
            return Tuple.Create(notFoundIndex, notFoundIndex);
        }

        var openingAngleBracketIndex = email.LastIndexOf('<');
        var closingAngleBracketIndex = openingAngleBracketIndex > -1 ? email.Length - 1 : notFoundIndex;

        var result = Tuple.Create(openingAngleBracketIndex, closingAngleBracketIndex);
        isEnclosed = result.Item1 > notFoundIndex && result.Item2 > notFoundIndex;
        return result;
    }

    public static bool IsValidEmailSubject(string subject)
    {
        return !Regex.IsMatch(subject, InvalidEmailSubjectCharsRegEx);
    }

    public static bool IsValidAgentName(string name)
    {
        return IsRegexMatch(name, AgentNameRegEx) && !IsRegexMatch(name, @"(?:\r\n|\n|\r|\t)");
    }

    private static bool IsRegexMatch(string input, string pattern)
    {
        if (string.IsNullOrWhiteSpace(input) || !Regex.IsMatch(input, pattern))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Accepts a string input value to be sanitized and returns true if the sanitation succeeded: false if it didn't.
    /// The sanitized string is returned as an out parameter.
    /// </summary>
    /// <param name="input">The string value to be sanitized.</param>
    /// <param name="exclusionPattern">The pattern that matches characters to be stripped out of the input string value.</param>
    /// <param name="result">The sanitized string value.</param>
    /// <param name="patterns"></param>
    /// <param name="replacement">(Optional) The string value used to replace the stripped out values.</param>
    /// <returns>Whether the string was sanitized or not.</returns>
    public static bool TrySanitize(string input, string replacement, out string result, params string[] patterns)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            result = string.Empty;
            return false;
        }

        var options = RegexOptions.None;
        var sanitized = string.Empty;

        foreach (var pattern in patterns)
        {
            if (string.IsNullOrEmpty(sanitized))
                sanitized = Regex.Replace(input, pattern, replacement, options);
            else
                sanitized = Regex.Replace(sanitized, pattern, replacement, options);
        }

        sanitized = Regex.Replace(sanitized, @"[ ]{2,}", " ", options);
        result = sanitized.RemoveEscapeCharactersAndTrim();

        return true;
    }
}