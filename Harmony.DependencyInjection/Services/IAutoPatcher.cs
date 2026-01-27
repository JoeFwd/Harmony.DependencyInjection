using HarmonyLib;

namespace Harmony.DependencyInjection.Services
{
    /// <summary>
    /// Auto‑patches all loaded assemblies using a <see cref="HarmonyLib.Harmony"/> instance.
    /// </summary>
    public interface IAutoPatcher
    {
        /// <summary>
        /// Invokes <c>Harmony.PatchAll</c> on every assembly returned by <c>AppDomain.CurrentDomain.GetAssemblies()</c>.
        /// </summary>
        /// <param name="harmony">The Harmony instance to use for auto‑patching.</param>
        void PatchAllLoadedAssemblies(HarmonyLib.Harmony harmony);
    }
}
