// This file is written imitating the source code of aspnetcore
// See https://github.com/dotnet/aspnetcore/blob/main/src/Components/Server/src/ProtectedBrowserStorage/ProtectedBrowserStorage.cs

using Microsoft.JSInterop;
using System.Text.Json;

namespace Ling.Blazor.Services;

/// <summary>
/// Provides mechanisms for storing and retrieving data in the browser's 'localStorage' collection.
/// <para>
/// This data will be scoped to the current user's browser, shared across all tabs. The data will persist across browser restarts.
/// </para>
/// See: <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/localStorage"/>.
/// </summary>
/// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
public sealed class LocalStorage(IJSRuntime jsRuntime) : BrowserStorage("localStorage", jsRuntime);

/// <summary>
/// Provides mechanisms for storing and retrieving data in the browser's 'sessionStorage' collection.
/// <para>
/// This data will be scoped to the current browser tab. The data will be discarded if the user closes the browser tab or closes the browser itself.
/// </para>
/// See: <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/sessionStorage"/>.
/// </summary>
/// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
public sealed class SessionStorage(IJSRuntime jsRuntime) : BrowserStorage("sessionStorage", jsRuntime);

/// <summary>
/// Provides mechanisms for storing and retrieving data in the browser storage.
/// </summary>
public abstract class BrowserStorage
{
    private readonly string _storeName;
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Constructs an instance of <see cref="BrowserStorage"/>.
    /// </summary>
    /// <param name="storeName">The name of the store in which the data should be stored.</param>
    /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
    private protected BrowserStorage(string storeName, IJSRuntime jsRuntime)
    {
        if (string.IsNullOrWhiteSpace(storeName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(storeName));
        }

        _storeName = storeName;
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <summary>
    /// Asynchronously stores the specified data.
    /// </summary>
    /// <typeparam name="TValue">The type of data to store.</typeparam>
    /// <param name="key">A <see cref="string"/> value specifying the name of the storage slot to use.</param>
    /// <param name="value">A MessagePack-serialized <typeparamref name="TValue"/> value to store.</param>
    /// <param name="cancellationToken">A cancellation token to signal the cancellation of the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the completion of the operation.</returns>
    public ValueTask SetAsync<TValue>(string key, TValue value, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(value);

        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        var data = Convert.ToBase64String(bytes);
        return _jsRuntime.InvokeVoidAsync($"{_storeName}.setItem", cancellationToken, key, data);
    }

    /// <summary>
    /// Asynchronously retrieves the specified data.
    /// </summary>
    /// <typeparam name="TValue">The type of data to retrieve.</typeparam>
    /// <param name="key">A <see cref="string"/> value specifying the name of the storage slot to use.</param>
    /// <param name="cancellationToken">A cancellation token to signal the cancellation of the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the completion of the operation.</returns>
    public async ValueTask<TValue?> GetAsync<TValue>(string key, CancellationToken cancellationToken = default)
    {
        var data = await _jsRuntime.InvokeAsync<string?>($"{_storeName}.getItem", cancellationToken, key);
        if (string.IsNullOrWhiteSpace(data))
        {
            return default;
        }

        try
        {
            var bytes = Convert.FromBase64String(data);
            return JsonSerializer.Deserialize<TValue>(bytes);
        }
        catch (Exception ex)
        {
            await _jsRuntime.InvokeVoidAsync("console.error", $"Failed to deserialize data for key: {key}\nException: {ex}");
            await _jsRuntime.InvokeVoidAsync("confirm", $"Failed to retrieve data stored with key: {key}");
            return default;
        }
    }

    /// <summary>
    /// Asynchronously deletes any data stored for the specified key.
    /// </summary>
    /// <param name="key">
    /// A <see cref="string"/> value specifying the name of the storage slot whose value should be deleted.
    /// </param>
    /// <returns>A <see cref="ValueTask"/> representing the completion of the operation.</returns>
    public ValueTask DeleteAsync(string key) => _jsRuntime.InvokeVoidAsync($"{_storeName}.removeItem", key);
}
