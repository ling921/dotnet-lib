using Ling.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ling.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add distributed cache service, use the 'Cache' node in the configuration, and use memory cache by default
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    public static IServiceCollection AddDistributedCache(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDistributedCache(delegate { });
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<CacheOptions>, CacheConfigureOptions>());

        return services;
    }

    /// <summary>
    /// Add distributed cache service, use memory cache by default.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="setupAction">Cache options initialization delegate.</param>
    public static IServiceCollection AddDistributedCache(this IServiceCollection services, Action<CacheOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        var redisOptions = new CacheOptions();
        setupAction.Invoke(redisOptions);
        services.Configure(setupAction);

        services.TryAddSingleton<IDistributedCache>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CacheOptions>>().Value;
            switch (options.Type)
            {
                case CacheType.Redis:
                    RedisCacheOptions redisCacheOptions = options;
                    return new RedisCache(Options.Create(redisCacheOptions));
                case CacheType.Memory:
                default:
                    var memoryDistributedCacheOptions = new MemoryDistributedCacheOptions();
                    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                    return new MemoryDistributedCache(Options.Create(memoryDistributedCacheOptions), loggerFactory);
            }
        });

        services.TryAddSingleton<ICache, DistributedCache>();

        return services;
    }
}
