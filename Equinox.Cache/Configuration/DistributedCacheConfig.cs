using System.Runtime.Serialization;
using Equinox.Ioc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Equinox.Cache.Configuration
{
    public partial class DistributedCacheConfig : IConfig
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CacheType CacheType { get; set; } = CacheType.Memory;

        public string ConnectionString { get; set; } = "127.0.0.1:6379,ssl=False";
    }

    public enum CacheType
    {
        [EnumMember(Value = "memory")] Memory,
        [EnumMember(Value = "redis")] Redis
    }
}