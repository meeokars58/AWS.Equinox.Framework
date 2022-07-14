namespace Equinox.Cache.Configuration
{
    /// <summary>
    /// Represents cache configuration parameters
    /// </summary>
    public class CacheConfig
    {
        /// <summary>
        /// Gets or sets the default cache time in seconds
        /// </summary>
        public int DefaultCacheTime { get; set; } = 60 * 60;

        /// <summary>
        /// Gets or sets the short term cache time in seconds
        /// </summary>
        public int ShortTermCacheTime { get; set; } = 3 * 60;
    }
}