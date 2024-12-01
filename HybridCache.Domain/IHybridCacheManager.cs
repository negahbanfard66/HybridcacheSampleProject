namespace HybridCache.Domain
{
    public interface IHybridCacheManager
    {
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null);
        Task RemoveAsync(string key);
    }
}
