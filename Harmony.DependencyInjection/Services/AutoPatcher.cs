using System;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Harmony.DependencyInjection.Services;

/// <summary>
///     Default implementation of <see cref="IAutoPatcher" />. It iterates all loaded assemblies and invokes
///     <c>Harmony.PatchAll</c>.
/// </summary>
internal sealed class AutoPatcher : IAutoPatcher
{
    private readonly ILogger<AutoPatcher> _logger;
    private readonly Assembly _patchAssembly;

    public AutoPatcher(ILogger<AutoPatcher> logger, IPatchAssemblyProvider patchAssemblyProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _patchAssembly = patchAssemblyProvider.PatchAssembly;
    }

    public void PatchAllLoadedAssemblies(HarmonyLib.Harmony harmony)
    {
        if (harmony == null) throw new ArgumentNullException(nameof(harmony));

        try
        {
            harmony.PatchAll(_patchAssembly);
            _logger.LogInformation("Auto‑patched assembly {Assembly}.", _patchAssembly.FullName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to auto‑patch assembly {Assembly}.", _patchAssembly.FullName);
        }
    }
}