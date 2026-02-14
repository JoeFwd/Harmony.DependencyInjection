using System;
using System.Collections.Generic;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.Logging;

namespace Harmony.DependencyInjection;

internal sealed class HarmonyPatcher : IHarmonyPatcher
{
    private readonly IAutoPatcher _autoPatcher;
    private readonly HarmonyLib.Harmony _harmony;
    private readonly ILogger<HarmonyPatcher> _logger;
    private readonly IPatchApplier _patchApplier;
    private readonly IPatchDiscovery _patchDiscovery;

    /// <summary>
    ///     Initializes a new instance of <see cref="HarmonyPatcher" />.
    /// </summary>
    /// <param name="logger">Logger for diagnostic output.</param>
    /// <param name="patchDiscovery">Service that discovers patches.</param>
    /// <param name="patchApplier">Applier that applies discovered patches.</param>
    /// <param name="autoPatcher">Auto‑patcher for already loaded assemblies.</param>
    /// <param name="patchAssemblyProvider">Provides the assembly containing the patches.</param>
    public HarmonyPatcher(
        ILogger<HarmonyPatcher> logger,
        IPatchDiscovery patchDiscovery,
        IPatchApplier patchApplier,
        IAutoPatcher autoPatcher,
        IPatchAssemblyProvider patchAssemblyProvider)
    {
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        if (patchDiscovery == null) throw new ArgumentNullException(nameof(patchDiscovery));
        if (patchApplier == null) throw new ArgumentNullException(nameof(patchApplier));
        if (autoPatcher == null) throw new ArgumentNullException(nameof(autoPatcher));
        if (patchAssemblyProvider == null) throw new ArgumentNullException(nameof(patchAssemblyProvider));
        _logger = logger;
        _patchDiscovery = patchDiscovery;
        _patchApplier = patchApplier;
        _autoPatcher = autoPatcher;
        var assembly = patchAssemblyProvider.PatchAssembly;

        // Use a safe harmony ID based on assembly name
        var harmonyId = $"{assembly.GetName().Name}.harmony";
        _harmony = new HarmonyLib.Harmony(harmonyId);
    }

    public void ApplyPatches()
    {
        try
        {
            IReadOnlyList<IPatch> patches = _patchDiscovery.Discover();
            _logger.LogInformation("Applying {Count} patches.", patches.Count);
            _patchApplier.Apply(patches, _harmony);
            _autoPatcher.PatchAllLoadedAssemblies(_harmony);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply patches.");
        }
    }
}