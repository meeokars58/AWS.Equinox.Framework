using Equinox.Cache.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Equinox.Cache.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static async void AddCache(this IServiceCollection services, IConfiguration? configuration)
        {
            services.AddMemoryCache();
            services.AddSingleton<ILocker, MemoryCacheManager>();
            services.AddSingleton<IStaticCacheManager, MemoryCacheManager>();
        }
    }
}