using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Harmony.DependencyInjection;

internal sealed class HarmonyPatcher : IDisposable, IHostedService
{
    private readonly IAutoPatcher _autoPatcher;
    private readonly HarmonyLib.Harmony _harmony;
    private readonly ILogger<HarmonyPatcher> _logger;
    private readonly IPatchApplier _patchApplier;
    private readonly IPatchDiscovery _patchDiscovery;

    /// <summary>
    /// Initializes a new instance of <see cref="HarmonyPatcher"/>.
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

    public void Dispose()
    {
        Remove();
    }

    /// <summary>
    /// Starts the hosted service by applying all discovered patches.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token (not used).</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Apply();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the hosted service by removing all applied patches.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token (not used).</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Remove();
        return Task.CompletedTask;
    }

    private void Apply()
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
            throw;
        }
    }

    /// <summary>
    /// Removes all patches applied by this <see cref="HarmonyPatcher"/> instance.
    /// </summary>
    private void Remove()
    {
        try
        {
            _harmony.UnpatchAll();
            _logger.LogInformation("All patches from Harmony id {HarmonyId} have been removed.", _harmony.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove patches for Harmony id {HarmonyId}.", _harmony.Id);
            throw;
        }
    }
}