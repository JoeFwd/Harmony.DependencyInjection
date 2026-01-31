using System.Collections.Generic;
using Harmony.DependencyInjection.Patches;

namespace Harmony.DependencyInjection.Services;

/// <summary>
///     Discovers <see cref="IPatch" /> implementations in the current assembly and registers any patch‑specific services.
/// </summary>
internal interface IPatchDiscovery
{
    /// <summary>
    ///     Returns all discovered patches.
    /// </summary>
    IReadOnlyList<IPatch> Discover();
}