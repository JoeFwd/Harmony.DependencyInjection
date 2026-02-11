using System;
using System.Collections.Generic;
using System.Linq;
using Harmony.DependencyInjection.Patches;
using Microsoft.Extensions.Logging;

namespace Harmony.DependencyInjection.Services;

/// <inheritdoc/>
internal sealed class PatchDiscovery : IPatchDiscovery
{
    private readonly ILogger<PatchDiscovery> _logger;
    private readonly List<IPatch> _patches;

    public PatchDiscovery(
                IEnumerable<IPatch> patches,
                ILogger<PatchDiscovery> logger)
    {
        if (patches == null) throw new ArgumentNullException(nameof(patches));
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        _patches = patches.ToList();
        _logger = logger;
    }

    public IReadOnlyList<IPatch> Discover()
    {
        List<IPatch> list = _patches.ToList();

        foreach (var patch in list)
            _logger.LogInformation(
                "Discovered patch {PatchType}.",
                patch.GetType().FullName);

        return list.AsReadOnly();
    }
}