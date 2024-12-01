using HybridCache.Domain;
using HybridCache.Infrastructure;
using HybridCache.Infrastructure.Caching;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHybridCaching(builder.Configuration.GetSection("RedisCacheSettings")["ConnectionString"]);

var app = builder.Build();

app.MapGet("/data/{key}", async (string key, IHybridCacheManager cacheService) =>
{
    return await cacheService.GetOrAddAsync(key, async () =>
    {
        // Simulate data fetch (e.g., from database)
        await Task.Delay(500);
        return new { Key = key, Value = "Sample Data" };
    });
});

app.UseAuthorization();

app.MapControllers();

app.Run();
