using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Harmony.DependencyInjection;

/// <summary>
///     Provides extension methods for registering Harmony patching services into an <see cref="IServiceCollection" />.
///     The registration also adds a <see cref="Microsoft.Extensions.Hosting.IHostedService" /> implementation that applies
///     patches at runtime.
///     Consequently, the consuming application must be running a host (e.g., a <c>HostBuilder</c> or <c>WebApplication</c>
///     ) for the patching to take effect.
/// </summary>
public static class HarmonyPatcherRegistration
{
    /// <summary>
    ///     Adds the necessary services for Harmony patch discovery and application to the specified
    ///     <paramref name="services" />.
    ///     This method delegates to the internal registration implementation.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which Harmony patching services will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection" /> allowing further chaining.</returns>
    public static IServiceCollection AddHarmonyPatching(
        this IServiceCollection services)
    {
        services.AddSingleton<IPatchDiscovery, PatchDiscovery>();
        services.AddSingleton<IPatchApplier, PatchApplier>();
        services.AddSingleton<IAutoPatcher, AutoPatcher>();
        services.AddSingleton<IPatchAssemblyProvider, PatchAssemblyProvider>();
        services.AddSingleton<IHarmonyPatcher, HarmonyPatcher>();
        return services;
    }
}