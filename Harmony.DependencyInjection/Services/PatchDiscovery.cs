using Harmony.DependencyInjection.Patches;
using Microsoft.Extensions.Logging;

namespace Harmony.DependencyInjection.Services
{
    public sealed class PatchDiscovery : IPatchDiscovery
    {
        private readonly IEnumerable<IPatch> _patches;
        private readonly ILogger<PatchDiscovery> _logger;

        public PatchDiscovery(
            IEnumerable<IPatch> patches,
            ILogger<PatchDiscovery> logger)
        {
            _patches = patches;
            _logger = logger;
        }

        public IReadOnlyList<IPatch> Discover()
        {
            var list = _patches.ToList();

            foreach (var patch in list)
            {
                _logger.LogInformation(
                    "Discovered patch {PatchType}.",
                    patch.GetType().FullName);
            }

            return list;
        }
    }
}