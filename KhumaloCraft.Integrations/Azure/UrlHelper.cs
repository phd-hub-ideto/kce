namespace KhumaloCraft.Integrations.Azure;

internal static class UrlHelper
{
    private static readonly string _azureUrl = "https://{0}.blob.core.windows.net/{1}";

    internal static string GetFormattedUrl(string storageAccount, string container)
    {
        return string.Format(_azureUrl, storageAccount, container);
    }
}