using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace KhumaloCraft.Domain.Authentication.AccessTokens;

[DataContract]
public class AccessToken
{
    public const string KEY = SecurityHelper.AccessTokenKey;

    public static readonly TimeSpan TTL = TimeSpan.FromDays(60);

    [DataMember(Name = "p")]
    public string PasswordHash { get; set; }

    [DataMember(Name = "i")]
    public DateTime IssueDate { get; set; }

    public static AccessToken TryDecode(string token)
    {
        try
        {
            string json = SecurityHelper.DecryptToken(token, SecurityHelper.StringEncoding.Base64, KEY);

            // trim nul chars, encryption algorithm padds the data @see SymmetricAlgorithm.Padding Property
            json = json.Trim('\0');

            var serializer = new DataContractJsonSerializer(typeof(AccessToken));

            using var stream = new MemoryStream(SecurityHelper.GetBytes(json));

            return serializer.ReadObject(stream) as AccessToken;
        }
        catch (FormatException)
        {
            // if the token is not valid, ignore it
            // someone probably just entered the wrong password ( @see KhumaloCraftPrincipal.ValidatePassword )
        }
        catch (Exception ex)
        {
            if (ex.Message == "Length of the data to decrypt is invalid.")
                return null; //probably wrong password
        }

        return null;
    }

    public static string Generate(byte[] passwordHash)
    {
        try
        {
            var serializer = new DataContractJsonSerializer(typeof(AccessToken));

            using var stream = new MemoryStream();

            serializer.WriteObject(stream, new AccessToken
            {
                PasswordHash = SecurityHelper.BytesToString(passwordHash),
                IssueDate = DateTime.Now.ToUniversalTime()
            });

            string json = SecurityHelper.BytesToString(stream.ToArray());

            return SecurityHelper.GenerateEncryptedToken(json, SecurityHelper.StringEncoding.Base64, KEY);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string Create(User user)
    {
        return Generate(user.PasswordHash);
    }

    public bool IsExpired { get { return DateTime.Now.ToUniversalTime().Subtract(IssueDate) > TTL; } }
}
