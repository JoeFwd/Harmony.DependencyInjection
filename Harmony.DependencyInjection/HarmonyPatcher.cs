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
        var assembly = patchAssemblyProvider.PatchAssembly;

        _harmony = new HarmonyLib.Harmony(
            $"{assembly.FullName}.harmony");
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

    private void Apply()
    {
        IReadOnlyList<IPatch> patches = _patchDiscovery.Discover();
        _patchApplier.Apply(patches, _harmony);
        _autoPatcher.PatchAllLoadedAssemblies(_harmony);
    }

    private void Remove()
    {
        _harmony.UnpatchAll(_harmony.Id);
        _logger.LogInformation("All patches from Harmony id {HarmonyId} have been removed.", _harmony.Id);
    }
}