using Newtonsoft.Json.Linq;

namespace Equinox.Cache.Configuration
{
    public class CacheSettings
    {
        public static CacheConfig CacheConfig { get; set; } = new CacheConfig();
        [Newtonsoft.Json.JsonExtensionData] public static IDictionary<string, JToken> AdditionalData { get; set; }
    }
}