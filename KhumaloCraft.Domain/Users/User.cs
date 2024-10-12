using KhumaloCraft.Domain.Security;
using KhumaloCraft.Helpers;

namespace KhumaloCraft.Domain.Users;

public sealed class User
{
    public int? Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MobileNumber { get; set; }
    public ImageHandle Image { get; set; } = ImageHandle.Create();
    public int? ImageReferenceId { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ActivatedDate { get; set; }
    public DateTime? ActivationEmailSentDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public bool ValidatedEmail { get; set; }
    public bool Deleted { get; set; }

    public string Fullname => FormattingHelper.AsFullName(FirstName, LastName);

    public string FriendlyName => string.IsNullOrWhiteSpace(FirstName) ? FormattingHelper.TitleCase(Username.Substring(0, Username.IndexOf('@'))) : FirstName;

    public bool IsNew => !Id.HasValue;

    /// <summary>
    /// A user is registered when they have an entry in the users table and are not marked as deleted.
    /// </summary>
    public bool IsRegistered => !Deleted;

    /// <summary>
    /// A user is validated when they are registered and have proved that their email belongs to them
    /// </summary>
    public bool IsValidated => IsRegistered && ValidatedEmail;

    /// <summary>
    /// A user can only be activated after their email has been validated. This is done after the password is set
    /// </summary>
    public bool IsActivated => IsRegistered && IsValidated && ActivatedDate.HasValue;

    public User() { }

    public User(string username)
    {
        Reset();

        Username = username;
    }

    public User(string username, string firstName, string lastName)
        : this(username)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public static User CreateNewUser(
        string email, string mobileNumber,
        string firstName, string lastName)
    {
        return new User(email, firstName, lastName)
        {
            MobileNumber = mobileNumber,
            Deleted = false,
            PasswordHash = SecurityHelper.InvalidHash
        };
    }

    public static User CreateNewUser(
        string email, string mobileNumber, string fullName)
    {
        string firstName;
        var lastName = string.Empty;

        var lastSpaceIndex = fullName.LastIndexOf(' ');

        if (lastSpaceIndex > -1)
        {
            firstName = fullName.Substring(0, lastSpaceIndex);
            lastName = fullName.Substring(lastSpaceIndex + 1, fullName.Length - (lastSpaceIndex + 1));
        }
        else
        {
            firstName = fullName;
        }

        return CreateNewUser(email, mobileNumber, firstName, lastName);
    }

    public void Reset()
    {
        SetPassword(string.Empty);
        ActivatedDate = null;
        FirstName = string.Empty;
        LastName = string.Empty;
        MobileNumber = string.Empty;
        ValidatedEmail = false;
        Deleted = false;

        // Password Hash is initialized to an invalid hash, indicating that there is no password set on the account.
        // Password Salt is automatically generated
        PasswordHash = SecurityHelper.InvalidHash;
        PasswordSalt = SecurityHelper.GenerateSalt();

        ImageReferenceId = null;

        //TODO-LP : Use ImageHandle
        //Image.SetReferenceId(null);
    }

    public void SetPassword(string password)
    {
        if (PasswordSalt.IsNullOrEmpty())
        {
            PasswordSalt = SecurityHelper.GenerateSalt();
        }

        PasswordHash = SecurityHelper.GenerateWeakHash(password, PasswordSalt);
    }

    public override string ToString()
    {
        return $"{Username} - {Id}";
    }
}