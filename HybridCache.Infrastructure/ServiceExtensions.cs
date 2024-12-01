using HybridCache.Domain;
using HybridCache.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace HybridCache.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void AddHybridCaching(this IServiceCollection services, string redisConnectionString)
        {
            // Add In-Memory Cache
            services.AddMemoryCache();

            // Add Distributed Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });

            services.AddScoped<IHybridCacheManager, HybridCacheManager>();
        }
    }
}
