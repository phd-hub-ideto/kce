using Newtonsoft.Json;
using KhumaloCraft.Helpers;
using System.Net;
using KhumaloCraft.Domain.Authentication;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Application.Cookies;
using KhumaloCraft.Application.Users;

namespace KhumaloCraft.Application.UserContext;

public class UserContext
{
    public static UserContext Current
    {
        get
        {
            if (!Dependencies.DependencyManager.HttpContextProvider.CanGetHttpContext)
                throw new InvalidOperationException("UserContext is only available in an HTTP Context");

            var current = Dependencies.DependencyManager.HttpContextProvider.GetItem("UserContext") as UserContext;

            if (current == null)
            {
                current = new UserContext();
                Dependencies.DependencyManager.HttpContextProvider.SetItem("UserContext", current);
            }

            return current;
        }
    }

    private UserContext()
    {
        UntrustedDetails = new PersonalDetails();

        Cookie = LoadCookie();

        if (PrincipalResolver.Instance.TryResolveCurrentUser(out var currentUser))
        {
            UntrustedDetails = new PersonalDetails(currentUser);

            TrustedUser = currentUser;
        }
        else
        {
            if (Cookie.User != null)
            {
                SetFullName(Cookie.User.FullName);

                UntrustedDetails.EmailAddress = Cookie.User.EmailAddress;

                UntrustedDetails.MobileNumber = Cookie.User.MobileNumber;
            }
        }

        ShowCountryDetectionDialog = Cookie.ShowCountryDetectionDialog;
        HideMapSearch = Cookie.HideMapSearch;

        if (Cookie.CityId != null)
        {
            LastVisitedLocation = new LastVisitedLocation(Cookie.CityId.Value, LastVisitedType.City);
        }
        else if (Cookie.SuburbId != null)
        {
            LastVisitedLocation = new LastVisitedLocation(Cookie.SuburbId.Value, LastVisitedType.Suburb);
        }
        else if (Cookie.ProvinceId != null)
        {
            LastVisitedLocation = new LastVisitedLocation(Cookie.ProvinceId.Value, LastVisitedType.Province);
        }

        RecentSearches = Cookie.RecentSearches;
    }

    private static UserContextCookie LoadCookie()
    {
        UserContextCookie userContextCookie;

        Dependencies.DependencyManager.HttpRequestProvider.TryGetCookie(Cookies.Cookie.User.Name(), out var cookie);

        if (cookie != null)
        {
            try
            {
                userContextCookie = JsonConvert.DeserializeObject<UserContextCookie>(WebUtility.UrlDecode(cookie));
                if (userContextCookie == null)
                {
                    userContextCookie = new UserContextCookie();
                }
            }
            catch
            {
                userContextCookie = new UserContextCookie();
            }
        }
        else
        {
            userContextCookie = new UserContextCookie();

            // Check for legacy cookies
            if (Dependencies.DependencyManager.HttpRequestProvider.TryGetCookie(Cookies.Cookie.UserDetails.Name(), out var _cookie))
            {
                var userDetails = JsonConvert.DeserializeObject<UserLegacyContextCookieUserDetails>(WebUtility.UrlDecode(_cookie));

                userContextCookie.User.FullName = userDetails.FullName ?? string.Empty;
                userContextCookie.User.EmailAddress = userDetails.EmailAddress ?? string.Empty;
                userContextCookie.User.MobileNumber = userDetails.MobileNumber ?? string.Empty;
            }
        }

        return userContextCookie;
    }

    public PersonalDetails UntrustedDetails { get; private set; }
    public bool? ShowCountryDetectionDialog { get; set; }
    public LastVisitedLocation LastVisitedLocation { get; set; }
    public bool? HideMapSearch { get; set; }

    public string FullName
        => ((UntrustedDetails.FirstName ?? string.Empty) + " " + (UntrustedDetails.LastName ?? string.Empty)).Trim();

    public bool IsRegistered
        => TrustedUser != null;

    public bool IsActivated
        => TrustedUser?.IsActivated == true;

    public RecentSearchBar RecentSearches { get; set; }

    public void SetFullName(string fullName)
    {
        if (fullName != null)
        {
            // Only Format the name if one of the parts has been left blank.
            if (string.IsNullOrWhiteSpace(UntrustedDetails.LastName) || string.IsNullOrWhiteSpace(UntrustedDetails.FirstName))
            {
                FormattingHelper.SplitNames(fullName, out var firstName, out var lastName);

                UntrustedDetails.FirstName = firstName;
                UntrustedDetails.LastName = lastName;
            }
        }
        else
        {
            UntrustedDetails.FirstName = null;
            UntrustedDetails.LastName = null;
        }
    }

    public void ClearUserInfoCookies()
    {
        Cookie.User = null;

        SaveCookie(Cookie);
    }

    private string _currentUserEmail;

    private User _currentUser;

    public User TrustedUser
    {
        get
        {
            if (_currentUserEmail != UntrustedDetails.EmailAddress)
            {
                _currentUserEmail = UntrustedDetails.EmailAddress;

                if (!string.IsNullOrWhiteSpace(_currentUserEmail))
                {
                    var userRepository = Dependencies.DependencyManager.Container.GetInstance<IUserRepository>();

                    _currentUser = userRepository.Query().SingleOrDefault(u => u.Username == _currentUserEmail);
                }
                else
                {
                    _currentUser = null;
                }
            }

            return _currentUser;
        }
        set
        {
            if (value != null)
            {
                _currentUserEmail = value.Username;
                _currentUser = value;
            }
            else
            {
                _currentUserEmail = null;
                _currentUser = null;
            }
        }
    }

    public bool BeginUserActivation(FullUriBuilder fullUriBuilder,
                                    string returnUrl = null,
                                    string returnUrlDescription = null)
    {
        if (EnsureUser() || !TrustedUser.IsActivated)
        {
            var accountManager = new UserAccountManager(TrustedUser);

            accountManager.SendActivation(fullUriBuilder, returnUrl, returnUrlDescription);

            return true;
        }

        return false;
    }

    public void SetPassword(string password)
    {
        if (EnsureUser())
        {
            TrustedUser.SetPassword(password);

            SaveUser(TrustedUser);
        }
    }

    public UserContextCookie Cookie { get; set; }

    private string MakeNull(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value;
    }

    private bool EnsureUser()
    {
        if (TrustedUser == null)
        {
            if (!ValidationHelper.IsValidEmailAddress(UntrustedDetails.EmailAddress))
            {
                throw new ArgumentException("Invalid e-mail address");
            }

            TrustedUser = User.CreateNewUser(
                            UntrustedDetails.EmailAddress,
                            UntrustedDetails.MobileNumber, FullName);

            SaveUser(TrustedUser);

            return true;
        }

        return false;
    }

    private static readonly JsonSerializerSettings _userCookieSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, };

    private void SaveCookie(UserContextCookie cookie)
    {
        var value = WebUtility.UrlEncode(JsonConvert.SerializeObject(cookie, _userCookieSerializerSettings));

        var currentCookie = HttpHelper.GetCurrentCookie(Cookies.Cookie.User.Name());

        // Only save the cookie if it changed.
        // When you save cookie it stops caching.
        // Only save the cookie if response headers haven't been written.
        if ((currentCookie == null || value != currentCookie)
            && !Dependencies.DependencyManager.HttpResponseProvider.HeadersWritten())
        {
            Dependencies.DependencyManager.HttpResponseProvider.SetCookie(cookieName: Cookies.Cookie.User.Name(), value: value, expiry: DateTime.Now.AddMonths(1));

            HttpHelper.AllowCookiesInResponse = true;
        }
    }

    private void SaveUser(User user)
    {
        var userRepository = Dependencies.DependencyManager.Container.GetInstance<IUserRepository>();

        userRepository.Upsert(user);
    }
}
