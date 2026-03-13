using Microsoft.Extensions.Caching.Hybrid;

namespace HybridApp.Services
{
    public class HybridCacheService : ICacheService
    {
        private readonly HybridCache _cache;
        public HybridCacheService(HybridCache cache) => _cache = cache;

        public async Task<string?> GetAsync(string key)
        {
            return await _cache.GetOrCreateAsync<string>(key, async _ => null);
        }

        public async Task SetAsync(string key, string value, TimeSpan ttl)
        {
            var options = new HybridCacheEntryOptions
            {
                Expiration = ttl
            };

            await _cache.SetAsync(key, value, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}


/*
 Configure HybridCache in Program.cs:
builder.Services.AddHybridCache();
builder.Services.AddScoped<ICacheService, HybridCacheService>();
 */