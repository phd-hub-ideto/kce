using System.Security.Cryptography;

namespace KhumaloCraft.Application.DataProtection;

internal static class DataProtectorAlgorithmHelper
{
    public static DataProtectorAlgorithm DefaultAlgorithm
    {
        get { return DataProtectorAlgorithm.Aes256Hmac512; }
    }

    public static DataProtectionAlgorithmsResult GetAlgorithms(
        DataProtectorAlgorithm algorithmId)
    {
        switch (algorithmId)
        {
            case DataProtectorAlgorithm.Aes256Hmac512:
                var result = new DataProtectionAlgorithmsResult()
                {
                    EncryptionAlgorithm = Aes.Create(),
                    SigningAlgorithm = new HMACSHA512(),
                    KeyDerivationIterationCount = 10000
                };

                result.EncryptionAlgorithm.KeySize = 256;

                return result;
            default:
                throw new ArgumentOutOfRangeException(nameof(algorithmId));
        }
    }
}