using HybridCache.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace HybridCache.Infrastructure.Caching
{
    public class HybridCacheManager : IHybridCacheManager
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        public HybridCacheManager(IMemoryCache memoryCache, IDistributedCache distributedCache)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null)
        {
            if (_memoryCache.TryGetValue(key, out T value))
            {
                return value;
            }


            var cachedData = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cachedData))
            {
                value = JsonSerializer.Deserialize<T>(cachedData);
                _memoryCache.Set(key, value, absoluteExpiration ?? TimeSpan.FromMinutes(5));
                return value;
            }

            value = await factory();

            if (value != null)
            {
                _memoryCache.Set(key, value, absoluteExpiration ?? TimeSpan.FromMinutes(5));
                await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(30)
                    });
            }

            return value;
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            await _distributedCache.RemoveAsync(key);
        }
    }
}
