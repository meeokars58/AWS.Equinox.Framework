using Equinox.Cache.Configuration;

namespace Equinox.Cache.Services
{
    public interface IStaticCacheManager : IDisposable
    {
        Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire);

        Task<T> GetAsync<T>(CacheKey key, Func<T> acquire);

        T Get<T>(CacheKey key, Func<T> acquire);
        
        Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters);
        
        Task SetAsync(CacheKey key, object data);

        Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters);

        Task ClearAsync();
        
    }
}