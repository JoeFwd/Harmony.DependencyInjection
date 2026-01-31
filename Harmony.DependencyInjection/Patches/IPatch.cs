using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Harmony.DependencyInjection.Patches
{
    /// <summary>
    /// Represents a Harmony patch definition. Implementations provide information about the target method to be patched,
    /// the method that implements the patch, and the type of patch (prefix, postfix, transpiler, etc.).
    /// </summary>
    public interface IPatch
    {
        /// <summary>
        /// Gets the <see cref="MethodInfo"/> of the method that will be patched.
        /// Returns <c>null</c> if the target method is not resolved at compile time.
        /// </summary>
        MethodInfo? TargetMethod { get; }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> of the method that implements the patch logic.
        /// </summary>
        MethodInfo? PatchMethod { get; }

        /// <summary>
        /// Gets the <see cref="PatchType"/> indicating the kind of Harmony patch (e.g., Prefix, Postfix, Transpiler).
        /// </summary>
        PatchType PatchType { get; }
    }
}
