using System.Collections.Generic;
using Harmony.DependencyInjection.Patches;
using Microsoft.Extensions.Logging;

namespace Harmony.DependencyInjection.Services
{
    /// <summary>
    /// Discovers <see cref="IPatch"/> implementations in the current assembly and registers any patch‑specific services.
    /// </summary>
    public interface IPatchDiscovery
    {
        /// <summary>
        /// Returns all discovered patches.
        /// </summary>
        IReadOnlyList<IPatch> Discover();
    }
}
