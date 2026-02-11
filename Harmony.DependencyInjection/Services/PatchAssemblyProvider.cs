using System.Reflection;

namespace Harmony.DependencyInjection.Services;

/// <inheritdoc/>
internal class PatchAssemblyProvider : IPatchAssemblyProvider
{
    public Assembly PatchAssembly => Assembly.GetCallingAssembly();
}