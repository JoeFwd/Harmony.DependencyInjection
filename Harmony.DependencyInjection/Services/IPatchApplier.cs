using System.Collections.Generic;
using System.Collections.Generic;
using HarmonyLib;
using Harmony.DependencyInjection.Patches;

namespace Harmony.DependencyInjection.Services
{
    /// <summary>
    /// Applies a collection of <see cref="IPatch"/> instances to a <see cref="HarmonyLib.Harmony"/> instance.
    /// </summary>
    public interface IPatchApplier
    {
        /// <summary>
        /// Applies each patch according to its <see cref="PatchType"/>.
        /// </summary>
        /// <param name="patches">The patches to apply.</param>
        /// <param name="harmony">The Harmony instance used for patching.</param>
        void Apply(IEnumerable<IPatch> patches, HarmonyLib.Harmony harmony);
    }
}
