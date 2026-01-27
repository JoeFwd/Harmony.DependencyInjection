using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Harmony.DependencyInjection.Services
{
    /// <summary>
    /// Default implementation of <see cref="IAutoPatcher"/>. It iterates all loaded assemblies and invokes <c>Harmony.PatchAll</c>.
    /// </summary>
    public sealed class AutoPatcher : IAutoPatcher
    {
        private readonly ILogger<AutoPatcher> _logger;

        public AutoPatcher(ILogger<AutoPatcher> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void PatchAllLoadedAssemblies(HarmonyLib.Harmony harmony)
        {
            if (harmony == null) throw new ArgumentNullException(nameof(harmony));
            var assembly = Assembly.GetCallingAssembly();

            try
            {
                harmony.PatchAll(Assembly.GetCallingAssembly());
                _logger.LogInformation("Auto‑patched assembly {Assembly}.", assembly.FullName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to auto‑patch assembly {Assembly}.", assembly.FullName);
            }
        }
    }
}
