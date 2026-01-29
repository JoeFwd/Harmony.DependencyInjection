using System.Reflection;
using Microsoft.Extensions.Logging;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.Hosting;

namespace Harmony.DependencyInjection;

/// <summary>
/// Handles the lifecycle of Harmony patches.
/// </summary>
public sealed class HarmonyPatcher : IDisposable, IHostedService
{
    private readonly HarmonyLib.Harmony _harmony;
    private readonly ILogger<HarmonyPatcher> _logger;
    private readonly IPatchDiscovery _patchDiscovery;
    private readonly IPatchApplier _patchApplier;
    private readonly IAutoPatcher _autoPatcher;
    private readonly Assembly _assembly;

    /// <summary>
    /// Creates a new patcher instance with injected dependencies.
    /// </summary>
    /// <param name="logger">Logger for this class.</param>
    /// <param name="patchDiscovery">Service that discovers patches.</param>
    /// <param name="patchApplier">Service that applies patches.</param>
    /// <param name="autoPatcher">Service that auto‑patches loaded assemblies.</param>
    public HarmonyPatcher(
        ILogger<HarmonyPatcher> logger,
        IPatchDiscovery patchDiscovery,
        IPatchApplier patchApplier,
        IAutoPatcher autoPatcher, IPatchAssemblyProvider patchAssemblyProvider)
    {
        _logger = logger;
        _patchDiscovery = patchDiscovery;
        _patchApplier = patchApplier;
        _autoPatcher = autoPatcher;
        _assembly = patchAssemblyProvider.PatchAssembly;

        _harmony = new HarmonyLib.Harmony(
            $"{_assembly.FullName}.harmony");
    }

    
    // Compatibility overload accepting a service provider and an external Harmony instance.
    /// <summary>
    /// Applies every discovered patch and then patches all loaded assemblies.
    /// </summary>
    public void Apply()
    {
        IReadOnlyList<IPatch> patches = _patchDiscovery.Discover();
        _patchApplier.Apply(patches, _harmony);
        _autoPatcher.PatchAllLoadedAssemblies(_harmony);
    }

    /// <summary>
    /// Removes every patch that this instance applied.
    /// </summary>
    public void Remove()
    {
        _harmony.UnpatchAll(_harmony.Id);
        _logger.LogInformation("All patches from Harmony id {HarmonyId} have been removed.", _harmony.Id);
    }

    public void Dispose()
    {
        Remove();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Apply();
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Remove();
        return Task.CompletedTask;
    }
}
