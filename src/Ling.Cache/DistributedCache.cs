using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ling.Cache;

/// <summary>
/// An implementation of the cache service.
/// </summary>
internal class DistributedCache : ICache
{
    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// The distributed cache service.
    /// </summary>
    protected readonly IDistributedCache Cache;

    /// <summary>
    /// Constructor, initialize the cache service.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="cache">The distributed cache service.</param>
    public DistributedCache(
        ILoggerFactory loggerFactory,
        IDistributedCache cache)
    {
        Logger = loggerFactory.CreateLogger(GetType());
        Cache = cache;
    }

    #region Synchronization methods

    /// <inheritdoc/>
    public virtual void Set<T>(string key, T value, DateTimeOffset expires) where T : notnull
    {
        if (value != null && expires >= DateTimeOffset.UtcNow)
        {
            Cache.SetString(key, Convert(value), new DistributedCacheEntryOptions { AbsoluteExpiration = expires });
        }
    }

    /// <inheritdoc/>
    public virtual void Set<T>(string key, T value, TimeSpan expires) where T : notnull
    {
        Set(key, value, DateTimeOffset.Now + expires);
    }

    /// <inheritdoc/>
    public virtual void Set<T>(string key, T value) where T : notnull
    {
        if (value != null)
        {
            Cache.SetString(key, Convert(value));
        }
    }

    /// <inheritdoc/>
    public virtual string? Get(string key)
    {
        return Cache.GetString(key);
    }

    /// <inheritdoc/>
    public virtual T? Get<T>(string key)
    {
        var jsonString = Get(key);
        return Resolve<T>(jsonString);
    }

    /// <inheritdoc/>
    public virtual string? GetAndRefresh(string key, DateTimeOffset expires)
    {
        var str = Get(key);
        if (!string.IsNullOrEmpty(str))
        {
            Set(key, str, expires);
        }

        return str;
    }

    /// <inheritdoc/>
    public virtual T? GetAndRefresh<T>(string key, DateTimeOffset expires)
    {
        var jsonString = GetAndRefresh(key, expires);
        return Resolve<T>(jsonString);
    }

    /// <inheritdoc/>
    public virtual string? GetAndRefresh(string key, TimeSpan expires)
    {
        var str = Get(key);
        if (!string.IsNullOrEmpty(str))
        {
            Set(key, str, expires);
        }

        return str;
    }

    /// <inheritdoc/>
    public virtual T? GetAndRefresh<T>(string key, TimeSpan expires)
    {
        var jsonString = GetAndRefresh(key, expires);
        return Resolve<T>(jsonString);
    }

    /// <inheritdoc/>
    public virtual string? GetAndRemove(string key)
    {
        var str = Get(key);
        Remove(key);
        return str;
    }

    /// <inheritdoc/>
    public virtual T? GetAndRemove<T>(string key)
    {
        var jsonString = GetAndRemove(key);
        return Resolve<T>(jsonString);
    }

    /// <inheritdoc/>
    public virtual void Remove(string key)
    {
        Cache.Remove(key);
    }

    #endregion Synchronization methods

    #region Asynchronous methods

    /// <inheritdoc/>
    public virtual Task SetAsync<T>(string key, T value, DateTimeOffset expires, CancellationToken cancellationToken = default) where T : notnull
    {
        if (value is null || expires < DateTimeOffset.UtcNow)
        {
            return Task.CompletedTask;
        }
        else
        {
            return Cache.SetStringAsync(key, Convert(value), new DistributedCacheEntryOptions { AbsoluteExpiration = expires }, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public virtual Task SetAsync<T>(string key, T value, TimeSpan expires, CancellationToken cancellationToken = default) where T : notnull
    {
        return SetAsync(key, value, DateTimeOffset.Now + expires, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : notnull
    {
        if (value is null)
        {
            return Task.CompletedTask;
        }
        else
        {
            return Cache.SetStringAsync(key, Convert(value), cancellationToken);
        }
    }

    /// <inheritdoc/>
    public virtual Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return Cache.GetStringAsync(key, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var jsonString = await GetAsync(key, cancellationToken);
        return Resolve<T>(jsonString);
    }

    /// <inheritdoc/>
    public virtual async Task<string?> GetAndRefreshAsync(string key, DateTimeOffset expires, CancellationToken cancellationToken = default)
    {
        var str = await GetAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(str))
        {
            await SetAsync(key, str, expires, cancellationToken);
        }

        return str;
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetAndRefreshAsync<T>(string key, DateTimeOffset expires, CancellationToken cancellationToken = default)
    {
        var jsonString = await GetAndRefreshAsync(key, expires, cancellationToken);
        return Resolve<T>(jsonString);
    }

    /// <inheritdoc/>
    public virtual async Task<string?> GetAndRefreshAsync(string key, TimeSpan expires, CancellationToken cancellationToken = default)
    {
        var str = await GetAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(str))
        {
            await SetAsync(key, str, expires, cancellationToken);
        }

        return str;
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetAndRefreshAsync<T>(string key, TimeSpan expires, CancellationToken cancellationToken = default)
    {
        var jsonString = await GetAndRefreshAsync(key, expires, cancellationToken);
        return Resolve<T>(jsonString);
    }

    /// <inheritdoc/>
    public virtual async Task<string?> GetAndRemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var str = await GetAsync(key, cancellationToken);
        await RemoveAsync(key, cancellationToken);
        return str;
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetAndRemoveAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var jsonString = await GetAndRemoveAsync(key, cancellationToken);
        return Resolve<T>(jsonString);
    }

    /// <inheritdoc/>
    public virtual Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return Cache.RemoveAsync(key, cancellationToken);
    }

    #endregion Asynchronous methods

    #region Private methods

    private static string Convert<T>(T value) where T : notnull
    {
        if (IsBaseType<T>())
        {
            return value.ToString()!;
        }

        return JsonSerializer.Serialize(value);
    }

    private static T? Resolve<T>(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        if (IsBaseType<T>())
        {
            return (T)System.Convert.ChangeType(json, typeof(T));
        }

        return JsonSerializer.Deserialize<T>(json);
    }

    private static bool IsBaseType(Type type)
    {
        return type.IsPrimitive || type == typeof(string);
    }

    private static bool IsBaseType<T>()
    {
        var type = typeof(T);
        return IsBaseType(type);
    }

    #endregion Private methods
}
