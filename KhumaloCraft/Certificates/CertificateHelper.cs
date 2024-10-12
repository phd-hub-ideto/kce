using System.Security.Cryptography.X509Certificates;

namespace KhumaloCraft.Certificates;

public static class CertificateHelper
{
    public static X509Certificate2 FindCertificate(string certThumbprint, StoreLocation currentUser)
    {
        using var x509Store = new X509Store(currentUser);

        x509Store.Open(OpenFlags.ReadOnly);

        return x509Store.Certificates
            .Find(
                X509FindType.FindByThumbprint,
                certThumbprint,
                validOnly: false)
            .OfType<X509Certificate2>()
            .FirstOrDefault();
    }
}