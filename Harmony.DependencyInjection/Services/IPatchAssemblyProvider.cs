using System.Reflection;

namespace Harmony.DependencyInjection.Services;

/// <summary>
/// Provides the assembly that contains patch definitions.
/// </summary>
internal interface IPatchAssemblyProvider
{
    /// <summary>
    /// Gets the assembly containing the patches.
    /// </summary>
    Assembly PatchAssembly { get; }
}