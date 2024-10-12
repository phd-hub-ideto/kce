using KhumaloCraft.Utilities;
using System.Text.RegularExpressions;

namespace KhumaloCraft;

//TODO-L: PM -> Clean up and use http abstractions http://stackoverflow.com/a/15112174
public static class BrowserHelper
{
    // https://www.opera.com/docs/history/
    public const int OPERA_MINI_MINIMUM_SPUI_VERSION = 15;

    // https://dev.opera.com/articles/opera-mini-request-headers/
    public static readonly Regex OperaMiniUserAgentRegex = new Regex(@"(?<opera>^.*[\(])(?<platform>.*?[;])(?<product>.*?[\/])(?<client>.*?[\/])(?<server>.*?[;])(?<nonsense> *.;.*\))(?<presto> *Presto\/.* *)(?<desktop> *Version\/.* *)", RegexOptions.Compiled);

    #region http://detectmobilebrowsers.com

    public static readonly Regex MobileDetectRegexB =
        new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    public static readonly Regex MobileDetectRegexBTablet =
        new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    public static readonly Regex MobileDetectRegexV =
        new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    #endregion

    private class UserAgentInfo
    {
        private const string USER_AGENT_ANDROID = "Android";
        private const string USER_AGENT_APPLE_WEB_KIT = "AppleWebKit";
        private const string USER_AGENT_BLACKBERRY = "BlackBerry";
        private const string USER_AGENT_BLACKBERRY_PLAYBOOK = "PlayBook";
        private const string USER_AGENT_BLACKBERRY_TEN = "BB10";
        private const string USER_AGENT_CHROME = "Chrome";
        private const string USER_AGENT_IPHONE = "iPhone";
        private const string USER_AGENT_IPOD = "iPod";
        private const string USER_AGENT_OPERA_MINI = "Opera Mini";
        private const string USER_AGENT_WINDOWS_PHONE = "Windows Phone";

        private string _userAgent;

        private bool? _isBlackBerry;
        private bool? _isFeaturePhone;
        private bool? _isMobile;
        private bool? _isNativeAndroidBrowser;
        private bool? _isOperaMini;
        private bool? _isPortable;
        private bool? _isSmartphone;

        public UserAgentInfo(string userAgent)
        {
            _userAgent = userAgent;
        }

        private static int GetOperaMiniClientMajorVersion(string userAgent)
        {
            try
            {
                var match = OperaMiniUserAgentRegex.Match(userAgent);

                if (!match.Success)
                    return 0;

                var clientVersion = (match.Groups["client"].Value ?? String.Empty).Replace("/", String.Empty).Trim();

                if (clientVersion.Length == 0)
                    return 0;

                var versionNumbers = clientVersion.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                if (versionNumbers.Length == 0)
                    return 0;

                if (!Int32.TryParse(versionNumbers[0], out int majorVersion))
                    return 0;

                return majorVersion;
            }
            catch
            {
                return 0;
            }
        }

        public bool IsBlackBerry
        {
            get
            {
                if (!_isBlackBerry.HasValue)
                    _isBlackBerry =
                        _userAgent.IndexOf(USER_AGENT_BLACKBERRY_PLAYBOOK, StringComparison.OrdinalIgnoreCase) != -1 ||
                        _userAgent.IndexOf(USER_AGENT_BLACKBERRY, StringComparison.OrdinalIgnoreCase) != -1;

                return _isBlackBerry.Value;
            }
        }

        public bool IsOperaMini
        {
            get
            {
                if (!_isOperaMini.HasValue)
                    _isOperaMini = _userAgent.IndexOf(USER_AGENT_OPERA_MINI, StringComparison.OrdinalIgnoreCase) != -1;

                return _isOperaMini.Value;
            }
        }

        public bool IsMobileButNotTablet
        {
            get
            {
                if (!_isMobile.HasValue)
                    _isMobile = IsPortable(false);

                return _isMobile.Value;
            }
        }

        public bool IsFeaturePhone
        {
            get
            {
                if (!_isFeaturePhone.HasValue)
                    _isFeaturePhone =
                        (!IsOperaMini && IsBlackBerry) ||
                        (IsOperaMini && GetOperaMiniClientMajorVersion(_userAgent) < OPERA_MINI_MINIMUM_SPUI_VERSION);

                return _isFeaturePhone.Value;
            }
        }

        public bool IsNativeAndroidBrowser
        {
            get
            {
                if (!_isNativeAndroidBrowser.HasValue)
                {
                    if (!IsMobileButNotTablet)
                    {
                        _isNativeAndroidBrowser = false;
                    }
                    else
                    {
                        var apple =
                            _userAgent.IndexOf(USER_AGENT_IPHONE, StringComparison.OrdinalIgnoreCase) != -1 ||
                            _userAgent.IndexOf(USER_AGENT_IPOD, StringComparison.OrdinalIgnoreCase) != -1;

                        var android =
                            _userAgent.IndexOf(USER_AGENT_ANDROID, StringComparison.OrdinalIgnoreCase) != -1 ||
                            _userAgent.IndexOf(USER_AGENT_APPLE_WEB_KIT, StringComparison.OrdinalIgnoreCase) != -1;

                        var chrome =
                            _userAgent.IndexOf(USER_AGENT_CHROME, StringComparison.OrdinalIgnoreCase) != -1;

                        _isNativeAndroidBrowser = !apple && android && !chrome && !IsOperaMini;
                    }
                }

                return _isNativeAndroidBrowser.Value;
            }
        }

        public bool IsSmartphone
        {
            get
            {
                if (!_isSmartphone.HasValue)
                {
                    var apple =
                        _userAgent.IndexOf(USER_AGENT_IPHONE, StringComparison.OrdinalIgnoreCase) != -1 ||
                        _userAgent.IndexOf(USER_AGENT_IPOD, StringComparison.OrdinalIgnoreCase) != -1;

                    var android =
                        _userAgent.IndexOf(USER_AGENT_ANDROID, StringComparison.OrdinalIgnoreCase) != -1;

                    var blackberry =
                        _userAgent.IndexOf(USER_AGENT_BLACKBERRY_TEN, StringComparison.OrdinalIgnoreCase) != -1;

                    var windows =
                        _userAgent.IndexOf(USER_AGENT_WINDOWS_PHONE, StringComparison.OrdinalIgnoreCase) != -1;

                    _isSmartphone =
                        (!IsOperaMini && (apple || android || windows || blackberry)) ||
                        (IsOperaMini && GetOperaMiniClientMajorVersion(_userAgent) >= OPERA_MINI_MINIMUM_SPUI_VERSION);
                }

                return _isSmartphone.Value;
            }
        }

        private bool IsPortable(bool tablet)
        {
            if (!_isPortable.HasValue)
            {
                if (tablet)
                    _isPortable = ((MobileDetectRegexBTablet.IsMatch(_userAgent) || MobileDetectRegexV.IsMatch(_userAgent.Truncate(4))));
                else
                    _isPortable = ((MobileDetectRegexB.IsMatch(_userAgent) || MobileDetectRegexV.IsMatch(_userAgent.Truncate(4))));
            }

            return _isPortable.Value;
        }

    }

    private static PriorityList<string, UserAgentInfo> _userAgentInfo = new(1000, TimeSpan.MaxValue);

    private static UserAgentInfo GetUserAgentInfo(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return null;

        if (!_userAgentInfo.TryGetValue(userAgent, out UserAgentInfo result,
            delegate (string key, ref SizeValuePair<UserAgentInfo> item)
            {
                item.Size = 1;
                item.Value = new UserAgentInfo(key);
                return true;
            }))
        {
            throw new NotImplementedException();
        }

        return result;
    }

    public static bool IsSmartphone(string userAgent)
    {
        var userAgentInfo = GetUserAgentInfo(userAgent);

        if (userAgentInfo == null)
            return false;

        if (!userAgentInfo.IsMobileButNotTablet)
            return false;

        return userAgentInfo.IsSmartphone;
    }

    public static bool IsFeaturePhone(string userAgent)
    {
        var userAgentInfo = GetUserAgentInfo(userAgent);

        if (userAgentInfo == null)
            return false;

        return userAgentInfo.IsFeaturePhone;
    }
}