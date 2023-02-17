using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Ling.Cache;

/// <summary>
/// The cache options configuration service.
/// </summary>
internal class CacheConfigureOptions : ConfigureFromConfigurationOptions<CacheOptions>
{
    public CacheConfigureOptions(IConfiguration config) : base(config.GetSection("Cache"))
    {
    }
}
