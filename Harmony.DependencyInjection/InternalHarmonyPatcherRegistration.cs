using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Harmony.DependencyInjection;

internal static class InternalHarmonyPatcherRegistration
{
    internal static IServiceCollection AddHarmonyPatching(
        this IServiceCollection services)
    {
        services.AddSingleton<IPatchDiscovery, PatchDiscovery>();
        services.AddSingleton<IPatchApplier, PatchApplier>();
        services.AddSingleton<IAutoPatcher, AutoPatcher>();
        services.AddSingleton<IPatchAssemblyProvider, PatchAssemblyProvider>();

        services.AddSingleton<IHostedService, HarmonyPatcher>();

        return services;
    }
}