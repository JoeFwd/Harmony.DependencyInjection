using System.Reflection;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Harmony.DependencyInjection;

public static class HarmonyServiceCollectionExtensions
{
    public static IServiceCollection AddHarmonyPatching(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();

        foreach (var type in assembly.DefinedTypes
                     .Where(t => typeof(IPatch).IsAssignableFrom(t)
                                 && !t.IsAbstract
                                 && t.IsClass))
        {
            services.AddTransient(typeof(IPatch), type);
        }

        services.AddSingleton<IPatchDiscovery, PatchDiscovery>();
        services.AddSingleton<IPatchApplier, PatchApplier>();
        services.AddSingleton<IAutoPatcher, AutoPatcher>();
        services.AddSingleton<HarmonyPatcher>();

        return services;
    }
}