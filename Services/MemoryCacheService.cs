using Microsoft.Extensions.Caching.Memory;

namespace HybridApp.Services
{
  

    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        public MemoryCacheService(IMemoryCache cache) => _cache = cache;

        public Task<string?> GetAsync(string key) =>
            Task.FromResult(_cache.TryGetValue(key, out string value) ? value : null);

        public Task SetAsync(string key, string value, TimeSpan ttl)
        {
            _cache.Set(key, value, ttl);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
