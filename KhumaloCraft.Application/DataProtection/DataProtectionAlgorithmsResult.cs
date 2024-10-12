using System.Security.Cryptography;

namespace KhumaloCraft.Application.DataProtection;

internal record DataProtectionAlgorithmsResult : IDisposable
{
    public SymmetricAlgorithm EncryptionAlgorithm { get; init; }
    public KeyedHashAlgorithm SigningAlgorithm { get; init; }
    public int KeyDerivationIterationCount { get; init; }

    public void Dispose()
    {
        EncryptionAlgorithm?.Clear();
        SigningAlgorithm?.Clear();
        EncryptionAlgorithm?.Dispose();
        SigningAlgorithm?.Dispose();
    }
}