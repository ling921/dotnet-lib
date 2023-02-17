using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace Ling.Cache;

/// <summary>
/// Represents cache configuration options.
/// </summary>
public class CacheOptions : RedisCacheOptions
{
    /// <summary>
    /// Gets or sets the type of cache.
    /// </summary>
    public CacheType Type { get; set; }
}

/// <summary>
/// Represents the type of cache.
/// </summary>
public enum CacheType
{
    /// <summary>
    /// Memory cache.
    /// </summary>
    Memory = 0,

    /// <summary>
    /// Redis cache.
    /// </summary>
    Redis = 1,
}
