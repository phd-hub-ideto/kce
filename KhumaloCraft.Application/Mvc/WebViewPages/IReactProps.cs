namespace KhumaloCraft.Application.Mvc.WebViewPages;

public interface IReactProps
{
    bool TryGetCacheKey(out string cacheKey);
    TimeSpan? CacheKeyDuration { get; }
}