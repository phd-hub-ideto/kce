using System.Globalization;
using System.Text.RegularExpressions;

namespace KhumaloCraft.Application.Cookies;

public static class SameSiteCookieHelper
{
    private static readonly Regex _iosRegex = new Regex(@"\(iP.+; CPU .*OS (\d+)[_\d]*.*\) AppleWebKit\/", RegexOptions.Compiled);
    private static readonly Regex _macOsxVersionRegex = new Regex(@"\(Macintosh;.*Mac OS X (\d+)_(\d+)[_\d]*.*\) AppleWebKit\/", RegexOptions.Compiled);
    private static readonly Regex _isSafariRegex = new Regex(@"Version\/.* Safari\/", RegexOptions.Compiled);
    private static readonly Regex _isMacEmbeddedRegex = new Regex(@"^Mozilla\/[\.\d]+ \(Macintosh;.*Mac OS X [_\d]+\) AppleWebKit\/[\.\d]+ \(KHTML, like Gecko\)$", RegexOptions.Compiled);
    private static readonly Regex _isChromiumBasedRegex = new Regex("Chrom(e|ium)", RegexOptions.Compiled);
    private static readonly Regex _chromiumVersionRegex = new Regex(@"Chrom[^ \/]+\/(\d+)[\.\d]*", RegexOptions.Compiled);
    private static readonly Regex _isUcBrowserRegex = new Regex(@"UCBrowser\/", RegexOptions.Compiled);
    private static readonly Regex _ucBrowserVersionRegex = new Regex(@"UCBrowser\/(\d+)\.(\d+)\.(\d+)[\.\d]*", RegexOptions.Compiled);

    //Taken from: https://github.com/AzureAD/microsoft-identity-web/blob/master/src/Microsoft.Identity.Web/CookiePolicyOptionsExtensions.cs
    /// <summary>
    /// Checks if the specified user agent supports "SameSite=None" cookies.
    /// </summary>
    /// <param name="userAgent">Browser user agent.</param>
    /// <remarks>
    /// Incompatible user agents include:
    /// <list type="bullet">
    /// <item>Versions of Chrome from Chrome 51 to Chrome 66 (inclusive on both ends).</item>
    /// <item>Versions of UC Browser on Android prior to version 12.13.2.</item>
    /// <item>Versions of Safari and embedded browsers on MacOS 10.14 and all browsers on iOS 12.</item>
    /// </list>
    /// Reference: https://www.chromium.org/updates/same-site/incompatible-clients.
    /// </remarks>
    /// <returns>True, if the user agent does not allow "SameSite=None" cookie; otherwise, false.</returns>
    public static bool DisallowsSameSiteNone(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return false;
        }

        return HasWebKitSameSiteBug() ||
            DropsUnrecognizedSameSiteCookies();

        bool HasWebKitSameSiteBug() =>
            IsIosVersion(12) ||
            (IsMacosxVersion(10, 14) &&
            (IsSafari() || IsMacEmbeddedBrowser()));

        bool DropsUnrecognizedSameSiteCookies()
        {
            if (IsUcBrowser())
            {
                return !IsUcBrowserVersionAtLeast(12, 13, 2);
            }

            return IsChromiumBased() &&
                IsChromiumVersionAtLeast(51) &&
                !IsChromiumVersionAtLeast(67);
        }

        bool IsIosVersion(int major)
        {
            // Extract digits from first capturing group.
            Match match = _iosRegex.Match(userAgent);
            return match.Groups[1].Value == major.ToString(CultureInfo.CurrentCulture);
        }

        bool IsMacosxVersion(int major, int minor)
        {
            // Extract digits from first and second capturing groups.
            Match match = _macOsxVersionRegex.Match(userAgent);
            if (match.Success)
            {
                return match.Groups[1].Value == major.ToString(CultureInfo.CurrentCulture) &&
                    match.Groups[2].Value == minor.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                return false;
            }
        }

        bool IsSafari()
        {
            return _isSafariRegex.IsMatch(userAgent) &&
                   !IsChromiumBased();
        }

        bool IsMacEmbeddedBrowser()
        {
            return _isMacEmbeddedRegex.IsMatch(userAgent);
        }

        bool IsChromiumBased()
        {
            return _isChromiumBasedRegex.IsMatch(userAgent);
        }

        bool IsChromiumVersionAtLeast(int major)
        {
            // Extract digits from first capturing group.
            Match match = _chromiumVersionRegex.Match(userAgent);
            if (match.Success)
            {
                int version = Convert.ToInt32(match.Groups[1].Value, CultureInfo.CurrentCulture);
                return version >= major;
            }
            else
            {
                return false;
            }
        }

        bool IsUcBrowser()
        {
            return _isUcBrowserRegex.IsMatch(userAgent);
        }

        bool IsUcBrowserVersionAtLeast(int major, int minor, int build)
        {
            // Extract digits from three capturing groups.
            Match match = _ucBrowserVersionRegex.Match(userAgent);
            if (match.Success)
            {
                int major_version = Convert.ToInt32(match.Groups[1].Value, CultureInfo.CurrentCulture);
                int minor_version = Convert.ToInt32(match.Groups[2].Value, CultureInfo.CurrentCulture);
                int build_version = Convert.ToInt32(match.Groups[3].Value, CultureInfo.CurrentCulture);
                if (major_version != major)
                {
                    return major_version > major;
                }

                if (minor_version != minor)
                {
                    return minor_version > minor;
                }

                return build_version >= build;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool AllowsSameSiteNone(string userAgent)
    {
        return !DisallowsSameSiteNone(userAgent);
    }
}