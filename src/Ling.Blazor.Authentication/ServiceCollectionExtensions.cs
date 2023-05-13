using Ling.Blazor.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ling.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add JWT authorization services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddJwtAuthorization<TAuthenticationService>(this IServiceCollection services)
        where TAuthenticationService : JwtAuthenticationServiceBase
    {
        return services.AddJwtAuthorization<TAuthenticationService>(delegate { });
    }

    /// <summary>
    /// Add JWT authorization services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="setupAction">An <see cref="Action{AuthOptions}"/> to configure the provided <see cref="JwtAuthOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddJwtAuthorization<TAuthenticationService>(
        this IServiceCollection services,
        Action<JwtAuthOptions> setupAction)
        where TAuthenticationService : JwtAuthenticationServiceBase
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        services.Configure<JwtAuthOptions>(setupAction);

        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
        services.AddScoped<IJwtTokenService>(sp => (JwtAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        services.TryAddScoped<TAuthenticationService>();
        services.AddScoped<IJwtAuthenticationService>(sp => sp.GetRequiredService<TAuthenticationService>());

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

        services.AddScoped<JwtAuthorizationMessageHandler>();

        services
            .AddHttpClient("ServerAPI", client => client.BaseAddress = new Uri(baseAddress))
            .AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

        return services;
    }
}
