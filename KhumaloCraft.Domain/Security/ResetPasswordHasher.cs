using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Dates;
using KhumaloCraft.Domain.Users;
using System.Security.Cryptography;

namespace KhumaloCraft.Domain.Security;

[Singleton(Contract = typeof(IResetPasswordHasher))]
public sealed class ResetPasswordHasher : IResetPasswordHasher
{
    private readonly IUserRepository _userRepository;
    private readonly IDateProvider _dateProvider;
    private const int NumIterations = 100000;
    private const int HoursTillExpiry = 48;
    private const short CurrentTokenVersion = 1;

    public ResetPasswordHasher(
        IUserRepository userRepository,
        IDateProvider dateProvider)
    {
        _userRepository = userRepository;
        _dateProvider = dateProvider;
    }

    public string GenerateResetToken(string token, string username, string returnUrl = null)
    {
        long expiryDate = _dateProvider.GetDate().AddHours(HoursTillExpiry).ToUnixEpoch();
        return GenerateResetToken(token, username, expiryDate, CurrentTokenVersion, returnUrl);

    }

    private string GenerateResetToken(string token, string username, long expiryDate, short? version, string returnUrl = null)
    {
        var expiryDateString = Convert.ToString(expiryDate);

        var inputString = GetConcatenatedString(username, expiryDateString, returnUrl, version);

        var user = _userRepository.Query().SingleOrDefault(u => u.Username == username);

        if (user != null)
        {
            EnsureUserHasPasswordSalt(user);

            //If no version is specified we can assume that it is version 1.
            var safeVersion = version ?? 1;

            switch (safeVersion)
            {
                case 1:
                    using (var pbkdf2 = new Rfc2898DeriveBytes(inputString, user.PasswordSalt, NumIterations, HashAlgorithmName.SHA1))
                    {
                        var resetHash = Convert.ToBase64String(pbkdf2.GetBytes(user.PasswordSalt.Length));

                        return $"{token}|{resetHash}|{expiryDateString}{GetTokenVersionString(version)}";
                    }
                default:
                    throw new NotImplementedException($"Version: {version}");
            }
        }

        return string.Empty;
    }

    //The password reset hasher requires valid PasswordSalt.
    //This change is a low-cost way of ensuring all accounts do have PasswordSalt setup, by creating it if it does not exist. 
    private void EnsureUserHasPasswordSalt(User user)
    {
        if (user.PasswordSalt.Length == 0)
        {
            user.PasswordSalt = SecurityHelper.GenerateSalt();

            _userRepository.Upsert(user);
        }
    }

    public bool IsValidToken(string username, string token, string returnUrl = null)
    {
        var user = _userRepository.Query().Single(u => u.Username == username);

        var tokenInfo = token.Split('|');

        if (tokenInfo.Length == 1)
        {
            return SecurityHelper.GenerateHashForResetPassword(user.PasswordSalt) == token;
        }

        var expiryDate = tokenInfo[2];
        var requestHash = tokenInfo[1];

        short? version = tokenInfo.Length == 4 ? Convert.ToInt16(tokenInfo[3]) : null;

        var resetPasswordResult = GenerateResetToken(tokenInfo[0], username, long.Parse(expiryDate), version, returnUrl);
        var generatedHash = resetPasswordResult.Split('|');

        var parsedDate = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(expiryDate));

        return generatedHash[1] == requestHash && parsedDate.DateTime > _dateProvider.GetDate();
    }

    private string GetConcatenatedString(string username, string expiryDate, string redirectUrl = null, short? version = null)
    {
        return $"{username}|{expiryDate}|{redirectUrl}{GetTokenVersionString(version)}";
    }

    private string GetTokenVersionString(short? version)
    {
        return version.HasValue ? $"|{version.Value}" : string.Empty;
    }
}