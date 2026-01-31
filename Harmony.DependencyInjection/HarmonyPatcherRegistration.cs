using Microsoft.Extensions.DependencyInjection;

namespace Harmony.DependencyInjection;

public static class HarmonyPatcherRegistration
{
    public static IServiceCollection AddHarmonyPatching(
        this IServiceCollection services)
    {
        return InternalHarmonyPatcherRegistration.AddHarmonyPatching(services);
    }
}