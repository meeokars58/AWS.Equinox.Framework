using System.Text.Json.Serialization;

namespace Equinox.Ioc
{
    public partial interface IConfig
    {
        [JsonIgnore] string Name => GetType().Name;
    }
}