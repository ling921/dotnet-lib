using Ling.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ling.Blazor.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Extension method to add browser storage services to the service collection.
    /// The services added are <see cref="LocalStorage"/> and <see cref="SessionStorage"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> so that multiple calls can be chained.</returns>
    public static IServiceCollection AddBrowserStorage(this IServiceCollection services)
    {
        services.TryAddScoped<LocalStorage>();
        services.TryAddScoped<SessionStorage>();

        return services;
    }
}
