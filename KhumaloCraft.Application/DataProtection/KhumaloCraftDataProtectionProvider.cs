using KhumaloCraft.Application.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace KhumaloCraft.Application.DataProtection;

public class KhumaloCraftDataProtectionProvider : IDataProtectionProvider
{
    private readonly ISettings _settings;

    public KhumaloCraftDataProtectionProvider(ISettings settings)
    {
        _settings = settings;
    }

    /// <inheritdoc />
    public IDataProtector CreateProtector(string purpose)
    {
        if (purpose == AuthenticationCookieTicketFormat.CookieDataProtectionPurpose)
        {
            return new KhumaloCraftDataProtector(() => _settings.CookieProtectionKey, null, purpose);
        }
        else
        {
            return new KhumaloCraftDataProtector(() => _settings.DataProtectionKey, null, purpose);
        }
    }

    public static void Add(IServiceCollection services)
    {
        services.AddSingleton<IDataProtectionProvider, KhumaloCraftDataProtectionProvider>();
    }

    private class KhumaloCraftDataProtector : IDataProtector
    {
        private readonly Func<string> _keyFunc;
        private readonly string[] _purposes;
        private readonly string _encryptionKeySecret;
        private readonly string _signingKeySecret;
        private readonly DataProtectorAlgorithm _defaultAlgorithm = DataProtectorAlgorithm.Aes256Hmac512;
        private readonly ConcurrentDictionary<KeyDerivationKey, byte[]> _keyDerivationDictionary = new ConcurrentDictionary<KeyDerivationKey, byte[]>();

        public KhumaloCraftDataProtector(Func<string> keyFunc, string[] originalPurposes, string newPurpose)
        {
            _keyFunc = keyFunc;
            _purposes = ConcatPurposes(originalPurposes, newPurpose);
            _encryptionKeySecret = string.Join(',', _purposes.Select(p => p + "Encryption"));
            _signingKeySecret = string.Join(',', _purposes.Select(p => p + "Signing"));
        }

        /// <inheritdoc />
        public IDataProtector CreateProtector(string purpose)
        {
            return new KhumaloCraftDataProtector(
                _keyFunc,
                _purposes,
                purpose
            );
        }

        /// <inheritdoc />
        public byte[] Protect(byte[] plaintext)
        {
            using var algorithms = DataProtectorAlgorithmHelper.GetAlgorithms(_defaultAlgorithm);

            var key = _keyFunc();

            // Derive a key for encryption from the master key
            algorithms.EncryptionAlgorithm.Key = DeriveEncryptionKey(key, algorithms.EncryptionAlgorithm, algorithms.KeyDerivationIterationCount);

            // When the underlying encryption class is created it has a random IV by default
            // So we don't need to do anything IV wise.

            byte[] cipherTextAndIV;
            // And encrypt
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, algorithms.EncryptionAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(plaintext);
                cs.FlushFinalBlock();
                var encryptedData = ms.ToArray();

                cipherTextAndIV = CombineByteArrays(algorithms.EncryptionAlgorithm.IV, encryptedData);
            }

            // Now get a signature for the data so we can detect tampering in situ.
            var signature = SignData(cipherTextAndIV, key, algorithms.EncryptionAlgorithm, algorithms.SigningAlgorithm, algorithms.KeyDerivationIterationCount);

            // Add the signature to the cipher text.
            var signedData = CombineByteArrays(signature, cipherTextAndIV);

            // Add our algorithm identifier to the combined signature and cipher text.
            var algorithmIdentifier = BitConverter.GetBytes((int)_defaultAlgorithm);
            byte[] output = CombineByteArrays(algorithmIdentifier, signedData);

            Array.Clear(plaintext, 0, plaintext.Length);

            return output;
        }

        /// <inheritdoc />
        public byte[] Unprotect(byte[] protectedData)
        {
            byte[] unprotectedText;

            var protectedDataAsSpan = new ReadOnlySpan<byte>(protectedData);
            var offset = 4;

            // Read the saved algorithm details and create instances of those algorithms.
            var algorithmIdentifier = (DataProtectorAlgorithm)BitConverter.ToInt32(protectedDataAsSpan.Slice(0, offset));
            using var algorithms = DataProtectorAlgorithmHelper.GetAlgorithms(algorithmIdentifier);

            // Now extract the signature
            var signature = protectedDataAsSpan.Slice(offset, algorithms.SigningAlgorithm.HashSize / 8);
            offset += signature.Length;

            // And finally grab the rest of the data
            var cipherTextAndIV = protectedDataAsSpan.Slice(offset, protectedData.Length - offset);

            var key = _keyFunc();

            // Check the signature before anything else is done to detect tampering and avoid oracles.
            var computedSignature = SignData(cipherTextAndIV.ToArray(), key, algorithms.EncryptionAlgorithm, algorithms.SigningAlgorithm, algorithms.KeyDerivationIterationCount);
            if (!SpansAreEqual(computedSignature, signature))
            {
                throw new CryptographicException("Invalid Signature.");
            }

            // The signature is valid, so now we can work on decrypting the data.
            var ivLength = algorithms.EncryptionAlgorithm.BlockSize / 8;
            // The IV is embedded in the cipher text, so we extract it out.
            var initializationVector = cipherTextAndIV.Slice(0, ivLength);
            // Then we get the encrypted data.
            var cipherText = cipherTextAndIV.Slice(ivLength, cipherTextAndIV.Length - ivLength);

            algorithms.EncryptionAlgorithm.Key = DeriveEncryptionKey(key, algorithms.EncryptionAlgorithm, algorithms.KeyDerivationIterationCount);
            algorithms.EncryptionAlgorithm.IV = initializationVector.ToArray();

            // Decrypt
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, algorithms.EncryptionAlgorithm.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherText);
                cs.FlushFinalBlock();

                unprotectedText = ms.ToArray();
            }

            return unprotectedText;
        }

        private byte[] SignData(byte[] cipherText, string masterKey, SymmetricAlgorithm symmetricAlgorithm, KeyedHashAlgorithm hashAlgorithm, int keyDerivationIterationCount)
        {
            hashAlgorithm.Key = DeriveSigningKey(masterKey, symmetricAlgorithm, keyDerivationIterationCount);
            byte[] signature = hashAlgorithm.ComputeHash(cipherText);
            hashAlgorithm.Clear();
            return signature;
        }

        private byte[] DeriveSigningKey(string key, SymmetricAlgorithm algorithm, int keyDerivationIterationCount) =>
            DeriveKey(new KeyDerivationKey(key, _signingKeySecret, keyDerivationIterationCount, algorithm.KeySize));

        private byte[] DeriveEncryptionKey(string key, SymmetricAlgorithm algorithm, int keyDerivationIterationCount) =>
            DeriveKey(new KeyDerivationKey(key, _encryptionKeySecret, keyDerivationIterationCount, algorithm.KeySize));

        private byte[] DeriveKey(KeyDerivationKey derivationKey) =>
            _keyDerivationDictionary.GetOrAdd(derivationKey, k =>
            {
                return KeyDerivation.Pbkdf2(
                    k.Secret,
                    Convert.FromBase64String(k.Key),
                    KeyDerivationPrf.HMACSHA512,
                    k.Iterations,
                    k.KeySize / 8
                );
            });

        private static byte[] CombineByteArrays(byte[] left, byte[] right)
        {
            byte[] output = new byte[left.Length + right.Length];
            Buffer.BlockCopy(left, 0, output, 0, left.Length);
            Buffer.BlockCopy(right, 0, output, left.Length, right.Length);

            return output;
        }

        private static string[] ConcatPurposes(string[] originalPurposes, string newPurpose)
        {
            if (originalPurposes != null && originalPurposes.Length > 0)
            {
                var newPurposes = new string[originalPurposes.Length + 1];
                Array.Copy(originalPurposes, 0, newPurposes, 0, originalPurposes.Length);
                newPurposes[originalPurposes.Length] = newPurpose;
                return newPurposes;
            }
            else
            {
                return new string[] { newPurpose };
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool SpansAreEqual(ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            if (a == default || b == default)
            {
                return false;
            }

            if (a == b)
            {
                return true;
            }

            bool areSame = true;
            for (int i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }

        private sealed class KeyDerivationKey
        {
            public string Key { get; }
            public string Secret { get; }
            public int Iterations { get; }
            public int KeySize { get; }

            public KeyDerivationKey(string key, string secret, int iterations, int keySize)
            {
                Key = key;
                Secret = secret;
                Iterations = iterations;
                KeySize = keySize;
            }

            public override bool Equals(object obj)
            {
                return obj is KeyDerivationKey other &&
                    Key == other.Key &&
                    Secret == other.Secret &&
                    Iterations == other.Iterations &&
                    KeySize == other.KeySize;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Key, Secret, Iterations, KeySize);
            }
        }
    }
}