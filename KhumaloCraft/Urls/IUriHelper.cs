namespace KhumaloCraft.Urls
{
    public interface IUriHelper
    {
        Uri UriOrDefault(string url, string @default = null);
        bool IsLocalFile(string url);
    }
}
