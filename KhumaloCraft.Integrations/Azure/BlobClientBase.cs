using KhumaloCraft.Certificates;
using Azure.Identity;
using Azure.Storage.Blobs;
using System.Security.Cryptography.X509Certificates;

namespace KhumaloCraft.Integrations.Azure;

public abstract class BlobClientBase(ISettings settings)
{
    protected readonly ISettings _settings = settings;

    private ClientCertificateCredential ClientCertificateCredential
    {
        get
        {
            var clientCertificate = CertificateHelper.FindCertificate(_settings.AzureAdCertThumbprint, StoreLocation.CurrentUser)
                ?? CertificateHelper.FindCertificate(_settings.AzureAdCertThumbprint, StoreLocation.LocalMachine)
                ?? throw new ArgumentException($"Certificate with thumbprint {_settings.AzureAdCertThumbprint} used for ActiveDirectory authentication is not available.");

            return new ClientCertificateCredential(
                _settings.AzureAdDirectoryId,
                _settings.AzureAdApplicationId,
                clientCertificate
            );
        }
    }

    protected BlobContainerClient ContainerClient()
    {
        return new BlobContainerClient(new Uri($"{_settings.AzureKCResourceConnectionString}"));
    }

    protected BlobContainerClient ContainerClient(string storageAccount, string containerName)
    {
        return new BlobContainerClient(
            new Uri(
                UrlHelper.GetFormattedUrl(
                    storageAccount,
                    containerName
                )
            ),
            ClientCertificateCredential
        );
    }
}