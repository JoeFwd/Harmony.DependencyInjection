using Harmony.DependencyInjection.Patches;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Harmony.DependencyInjection.Services
{
    /// <summary>
    /// Default implementation of <see cref="IPatchApplier"/>. Mirrors the logic that was previously
    /// embedded in <see cref="HarmonyPatcher.Apply"/>.
    /// </summary>
    public sealed class PatchApplier : IPatchApplier
    {
        private readonly ILogger<PatchApplier> _logger;

        public PatchApplier(ILogger<PatchApplier> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Applies a collection of patches to the provided Harmony instance.
        /// </summary>
        /// <param name="patches">The collection of patches to apply. If <c>null</c>, an <see cref="ArgumentNullException"/> is thrown.</param>
        /// <param name="harmony">The Harmony instance used for patching. If <c>null</c>, an <see cref="ArgumentNullException"/> is thrown.</param>
        public void Apply(IEnumerable<IPatch> patches, HarmonyLib.Harmony harmony)
        {
            if (patches == null) throw new ArgumentNullException(nameof(patches));
            if (harmony == null) throw new ArgumentNullException(nameof(harmony));
        
            // Ensure thread‑safety when multiple callers apply patches concurrently.
            lock (harmony)
            {
                foreach (var patch in patches)
                {
                    if (patch.TargetMethod == null || patch.PatchMethod == null)
                    {
                        _logger.LogWarning("Patch {Patch} is missing TargetMethod or PatchMethod.", patch.GetType().FullName);
                        continue;
                    }
        
                    try
                    {
                        switch (patch.PatchType)
                        {
                            case PatchType.Transpiler:
                                harmony.Patch(patch.TargetMethod,
                                    transpiler: new HarmonyMethod(patch.PatchMethod));
                                break;
                            case PatchType.Prefix:
                                harmony.Patch(patch.TargetMethod,
                                    new HarmonyMethod(patch.PatchMethod));
                                break;
                            case PatchType.Postfix:
                                harmony.Patch(patch.TargetMethod,
                                    postfix: new HarmonyMethod(patch.PatchMethod));
                                break;
                            default:
                                _logger.LogError("Unknown PatchType {PatchType} for patch {Patch}.",
                                    patch.PatchType, patch.GetType().FullName);
                                continue;
                        }
        
                        _logger.LogInformation("Successfully applied {Patch} ({PatchType}).", patch.GetType().FullName, patch.PatchType);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to apply patch {Patch}.", patch.GetType().FullName);
                    }
                }
            }
        }
    }
}
