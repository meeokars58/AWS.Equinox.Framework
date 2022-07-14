using Equinox.Cache.Configuration;

namespace Equinox.Cache.Services
{
    /// <summary>
    /// Represents default values related to caching entities
    /// </summary>
    public static partial class MbisEntityCacheDefaults<T> where T : class
    {
        public static string TypeName => typeof(T).Name.ToLowerInvariant();
        public static CacheKey AllCacheKey => new($"Equinox.{TypeName}.all.", AllPrefix, Prefix);
        public static string Prefix => $"Equinox.{TypeName}.";
        public static string AllPrefix => $"Equinox.{TypeName}.all.";

        public static string ByIdsPrefix => $"Equinox.{TypeName}.byids.";
        public static string ByIdPrefix => $"Equinox.{TypeName}.byid.";

        public static CacheKey ByIdCacheKey => new CacheKey($"Equinox.{TypeName}.byid.{{0}}", ByIdPrefix, Prefix);
    }
}