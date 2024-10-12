using KhumaloCraft.Data.Entities;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Utilities;
using System.Security.Cryptography;
using System.Text;

namespace KhumaloCraft.Domain.Security;

public static class SecurityHelper
{
    public const string KhumaloCraftURLEncryptionKey = "RDSC3AN8WQXK8Z96YYDS8SQXQ3RYVEQ2";
    public const string KCEAuthenticationEncryptionKey = "K2Z9YPXAQ3D3KPVSW785PPW7DB57G5YM";
    public const string AccessTokenKey = "The Key Mst Be Exactly This Long"; // 32 chars
    public const int NUM_ITERATIONS = 100000;

    private const string InitializationVector = "KCE EncryptionIV";

    public enum StringEncoding
    {
        Base32,
        Base64,
    }

    private const char DefaultTokenPieceDelimiter = '-';

    /// <summary>
    /// An array of eight zero-bytes (that will never match a hashed password),
    /// used as a password hash on user accounts for which no password has been set.
    /// </summary>
    /// <remarks>
    /// Used by automatic account creation tools and the administrative reset-password function.
    /// In both cases, the user is sent an email with a link to set a valid password.
    /// </remarks>
    public static readonly byte[] InvalidHash = { 0, 0, 0, 0, 0, 0, 0, 0, };

    private static string _privateKeyForPasswordResetToken = "resetpassword";

    public static byte[] GenerateSalt()
    {
        var rng = RandomNumberGenerator.Create();
        var salt = new byte[8];

        rng.GetBytes(salt);

        return salt;
    }

    private static readonly byte[] _staticSalt = { byte.MaxValue, byte.MinValue, 13, 13, 13, 13, byte.MaxValue, byte.MinValue, };

    public static string GenerateStaticHash(string stringKeyForHash)
    {
        return GenerateHashForStaticSalt(stringKeyForHash, _staticSalt);
    }

    public static byte[] GenerateWeakHash(string password, byte[] salt)
    {
        using (var sha1 = SHA1.Create())
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            sha1.TransformBlock(passwordBytes, 0, passwordBytes.Length, passwordBytes, 0);

            sha1.TransformFinalBlock(salt, 0, salt.Length);

            return sha1.Hash;
        }
    }

    public static string GenerateHashForResetPassword(byte[] salt)
    {
        return GenerateBase64Hash(_privateKeyForPasswordResetToken, salt);
    }

    private static string GenerateHashForStaticSalt(string password, byte[] salt)
    {
        return GenerateBase64Hash(password, salt);
    }

    public static string GenerateBase64Hash(string password, byte[] salt)
    {
        var data = GenerateWeakHash(password, salt);

        var base64 = Convert.ToBase64String(data);

        return base64;
    }

    public static IList<Permission> GetGrantablePermissions(SecurityEntityType securityEntityType)
    {
        var grantablePermissions = new List<Permission>();

        foreach (var field in typeof(Permission).GetFields())
        {
            if (field.GetCustomAttributes(typeof(GrantOnAttribute), false).Cast<GrantOnAttribute>()
                .Any(attribute => attribute.SecurityEntityType == securityEntityType))
            {
                grantablePermissions.Add((Permission)field.GetRawConstantValue());
            }
        }

        return grantablePermissions.AsReadOnly();
    }

    public static string CreateValidationChecksum(object[] values, byte[] salt)
    {
        var fields = from value in values
                     where value != null
                     select value;

        return GenerateBase64Hash(string.Join(";", fields), salt);
    }

    public static void AddRolesToUser(IUserRoleRepository userRoleRepository, int userId, SecurityEntityType type, params int[] roleIds)
    {
        var roles = userRoleRepository.Query().Where(ur => ur.UserId == userId);

        using (var scope = DalScope.Begin())
        {
            foreach (var roleId in roleIds)
            {
                var newRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    SecurityEntityType = type
                };

                if (roles.Contains(newRole))
                    continue;

                userRoleRepository.Upsert(newRole);
            }

            scope.Commit();
        }
    }

    public static void DeleteRolesFromUser(IUserRoleRepository userRoleRepository, int userId, SecurityEntityType type, params int[] roleIds)
    {
        var roles = userRoleRepository.Query().Where(ur => ur.UserId == userId);

        using (var scope = DalScope.Begin())
        {
            foreach (var roleId in roleIds)
            {
                var specificRole = roles.FirstOrDefault(r =>
                    r.RoleId == roleId &&
                    r.SecurityEntityTypeId == (int)type);

                if (specificRole == null)
                    continue;

                userRoleRepository.Delete(specificRole.Id.Value);
            }

            scope.Commit();
        }
    }

    public static string GenerateEncryptedToken(string key, StringEncoding encoding, char delimiter, params string[] valuesToEncrypt)
    {
        var plainText = string.Join(delimiter.ToString(), valuesToEncrypt);

        return GenerateEncryptedToken(plainText, encoding, key);
    }

    public static string GenerateEncryptedToken(string key, StringEncoding encoding, params string[] valuesToEncrypt)
    {
        return GenerateEncryptedToken(key, encoding, DefaultTokenPieceDelimiter, valuesToEncrypt);
    }

    /// <summary>
    /// Encrypts plain text into an encoded string, using PropIQ's encryption routine. Suitable for use in URLs.
    /// </summary>
    /// <param name="plainText">The text to be encrypted</param>
    /// <param name="encoding">The type of encoding to use for encrypted data</param>
    public static string GenerateEncryptedToken(string plainText, StringEncoding encoding, string key)
    {
        byte[] data = Encoding.UTF8.GetBytes(plainText);

        using var rijn = Aes.Create();

        using var ms = new MemoryStream();

        byte[] rgbIV = Encoding.UTF8.GetBytes(InitializationVector);

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        using (var cs = new CryptoStream(ms, rijn.CreateEncryptor(keyBytes, rgbIV), CryptoStreamMode.Write))
        {
            cs.Write(data, 0, data.Length);
        }

        if (encoding == StringEncoding.Base64)
        {
            return Convert.ToBase64String(ms.ToArray());
        }

        if (encoding == StringEncoding.Base32)
        {
            var bytes = ByteArrayUtils.Transform(ms.ToArray(), DataTransformType.ToBase32);

            return Encoding.Default.GetString(bytes).ToLower();
        }

        return null;
    }

    public static string[] DecryptToken(string token, string key, StringEncoding encoding, char delimiter)
    {
        return DecryptToken(token, encoding, key).Split(new[] { delimiter }, StringSplitOptions.None);
    }

    /// <summary>
    /// Decrypts a token that was encrypted using <see cref="GenerateEncryptedToken"/>.
    /// </summary>
    /// <param name="token">The token to be decrypted</param>
    /// <param name="encoding">The token's encoding</param>
    /// <param name="key">Decryption key to use</param>
    /// <returns></returns>
    public static string DecryptToken(string token, StringEncoding encoding, string key)
    {
        byte[] encryptedData = null;

        if (encoding == StringEncoding.Base64)
        {
            encryptedData = Convert.FromBase64String(token);
        }
        else if (encoding == StringEncoding.Base32)
        {
            encryptedData = Encoding.Default.GetBytes(token.ToUpper());

            encryptedData = ByteArrayUtils.Transform(encryptedData, DataTransformType.FromBase32);
        }

        using (var rijn = Aes.Create())
        {
            using (var ms = new MemoryStream(encryptedData))
            {
                byte[] rgbIV = Encoding.UTF8.GetBytes(InitializationVector);
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                using (var cs = new CryptoStream(ms, rijn.CreateDecryptor(keyBytes, rgbIV), CryptoStreamMode.Read))
                {
                    var buffer = cs.ReadAllBytes();

                    return Encoding.UTF8.GetString(buffer)?.TrimEnd('\0');
                }
            }
        }
    }

    public static bool TryDecryptToken(string token, StringEncoding encoding, string key, out string result)
    {
        try
        {
            result = DecryptToken(token, encoding, key);

            return true;
        }
        catch (CryptographicException)
        {
            result = string.Empty;

            return false;
        }
    }

    public static byte[] GenerateEncryptedToken(RSACryptoServiceProvider rsaPublicKey, string dataToEncrypt)
    {
        if (rsaPublicKey == null)
        {
            throw new Exception("Certificate was not found and asymmetric encryption could not be done.");
        }

        using (rsaPublicKey)
        {
            var rsaParams = rsaPublicKey.ExportParameters(false);

            //get the max data size according to http://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider.encrypt.aspx
            var maxDataSize = rsaParams.Modulus.Length - 11;

            if (dataToEncrypt.Length <= maxDataSize)
            {
                var data = Encoding.UTF8.GetBytes(dataToEncrypt);

                return rsaPublicKey.Encrypt(data, false);
            }

            throw new Exception("Data size too large to encrypt using RSA.");
        }
    }

    public static string DecryptToken(RSACryptoServiceProvider rsaPrivateKey, byte[] token)
    {
        using (rsaPrivateKey)
        {
            return Encoding.UTF8.GetString(rsaPrivateKey.Decrypt(token, false));
        }
    }

    public static byte[] GetBytes(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    public static string BytesToString(byte[] data)
    {
        return Encoding.UTF8.GetString(data);
    }
}
