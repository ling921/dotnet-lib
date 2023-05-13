namespace Ling.Cache;

/// <summary>
/// Represents the interface of the cache service.
/// </summary>
public interface ICache
{
    #region Synchronization methods

    /// <summary>
    /// Sets the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="expires">The expiration time for the value.</param>
    void Set<T>(string key, T value, DateTimeOffset expires) where T : notnull;

    /// <summary>
    /// Sets the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="expires">The expiration time for the value.</param>
    void Set<T>(string key, T value, TimeSpan expires) where T : notnull;

    /// <summary>
    /// Sets the non-expiring value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    void Set<T>(string key, T value) where T : notnull;

    /// <summary>
    /// Gets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns>The located value or null.</returns>
    string? Get(string key);

    /// <summary>
    /// Gets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns>The located value or null.</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Gets a value with the given key, then sets the expiration time.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="expires">The new expiration time for the value.</param>
    /// <returns>The located value or null.</returns>
    string? GetAndRefresh(string key, DateTimeOffset expires);

    /// <summary>
    /// Gets a value with the given key, then sets the expiration time.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="expires">The new expiration time for the value.</param>
    /// <returns>The located value or null.</returns>
    T? GetAndRefresh<T>(string key, DateTimeOffset expires);

    /// <summary>
    /// Gets a value with the given key, then sets the expiration time.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="expires">The new expiration time for the value.</param>
    /// <returns>The located value or null.</returns>
    string? GetAndRefresh(string key, TimeSpan expires);

    /// <summary>
    /// Gets a value with the given key, then sets the expiration time.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="expires">The new expiration time for the value.</param>
    /// <returns>The located value or null.</returns>
    T? GetAndRefresh<T>(string key, TimeSpan expires);

    /// <summary>
    /// Gets a value with the given key, then removes the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns>The located value or null.</returns>
    string? GetAndRemove(string key);

    /// <summary>
    /// Gets a value with the given key, then removes the value with the given key.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns>The located value or null.</returns>
    T? GetAndRemove<T>(string key);

    /// <summary>
    /// Removes the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    void Remove(string key);

    #endregion Synchronization methods

    #region Asynchronous methods

    /// <summary>
    /// Sets the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="expires">The expiration time for the value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, DateTimeOffset expires, CancellationToken token = default) where T : notnull;

    /// <summary>
    /// Sets the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="expires">The expiration time for the value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan expires, CancellationToken token = default) where T : notnull;

    /// <summary>
    /// Sets the non-expiring value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set in the cache.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, CancellationToken token = default) where T : notnull;

    /// <summary>
    /// Gets a value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the located
    /// value or null.
    /// </returns>
    Task<string?> GetAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Gets a value with the given key.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the located
    /// value or null.
    /// </returns>
    Task<T?> GetAsync<T>(string key, CancellationToken token = default);

    /// <summary>
    /// Gets a value with the given key, then sets the expiration time.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="expires">The new expiration time for the value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the located
    /// value or null.
    /// </returns>
    Task<string?> GetAndRefreshAsync(string key, DateTimeOffset expires, CancellationToken token = default);

    /// <summary>
    /// Gets a value with the given key, then sets the expiration time.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="expires">The new expiration time for the value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the located
    /// value or null.
    /// </returns>
    Task<T?> GetAndRefreshAsync<T>(string key, DateTimeOffset expires, CancellationToken token = default);

    /// <summary>
    /// Gets a value with the given key, then sets the expiration time.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="expires">The new expiration time for the value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the located
    /// value or null.
    /// </returns>
    Task<string?> GetAndRefreshAsync(string key, TimeSpan expires, CancellationToken token = default);

    /// <summary>
    /// Gets a value with the given key, then sets the expiration time.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="expires">The new expiration time for the value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the located
    /// value or null.
    /// </returns>
    Task<T?> GetAndRefreshAsync<T>(string key, TimeSpan expires, CancellationToken token = default);

    /// <summary>
    /// Gets a value with the given key, then sets the expiration time.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the located
    /// value or null.
    /// </returns>
    Task<string?> GetAndRemoveAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Gets a value with the given key, then removes the value with the given key.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the located
    /// value or null.
    /// </returns>
    Task<T?> GetAndRemoveAsync<T>(string key, CancellationToken token = default);

    /// <summary>
    /// Removes the value with the given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="token">
    /// Optional. The <see cref="CancellationToken"/> used to propagate notifications that the
    /// operation should be canceled.
    /// </param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task RemoveAsync(string key, CancellationToken token = default);

    #endregion Asynchronous methods
}
