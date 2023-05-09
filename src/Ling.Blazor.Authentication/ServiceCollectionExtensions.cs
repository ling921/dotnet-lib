using Ling.Blazor.Authentication.Internal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Ling.Blazor.Authentication;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add jwt authorization services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddJwtAuthorization<TAuthenticationService>(this IServiceCollection services)
        where TAuthenticationService : AuthenticationServiceBase
    {
        return services.AddJwtAuthorization<TAuthenticationService>(delegate { });
    }

    /// <summary>
    /// Add jwt authorization services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="setupAction">An <see cref="Action{AuthOptions}"/> to configure the provided <see cref="AuthOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddJwtAuthorization<TAuthenticationService>(
        this IServiceCollection services,
        Action<AuthOptions> setupAction)
        where TAuthenticationService : AuthenticationServiceBase
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        services.Configure<AuthOptions>(setupAction);

        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();
        services.AddScoped<ITokenService>(sp => (AppAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        services.AddScoped<IAuthenticationService, TAuthenticationService>();

        return services;
    }

    /// <summary>
    /// Add <see cref="HttpClient"/> service for requesting server API.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="baseAddress">The base address of the API server.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UriFormatException"></exception>
    public static IServiceCollection AddServerAPI(this IServiceCollection services, string baseAddress)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(baseAddress);

        services.AddScoped<AppAuthorizationMessageHandler>();

        services
            .AddHttpClient("ServerAPI", client => client.BaseAddress = new Uri(baseAddress))
            .AddHttpMessageHandler<AppAuthorizationMessageHandler>();

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

        return services;
    }
}
