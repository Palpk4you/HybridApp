namespace HybridApp.Services
{
    public interface ICacheService
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value, TimeSpan ttl);
        Task RemoveAsync(string key);
    }
}

/*
 *Swapping Implementations for InMemory, Redis, or HybridCache
- In development:
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
- In production with Redis:
builder.Services.AddScoped<ICacheService, RedisCacheService>();
- In .NET 9/10 with HybridCache:
builder.Services.AddScoped<ICacheService, HybridCacheService>();


Your AuthController and middleware don’t change — they only depend on ICacheService.

 */