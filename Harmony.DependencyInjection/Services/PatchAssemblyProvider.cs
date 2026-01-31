using System.Reflection;

namespace Harmony.DependencyInjection.Services;

internal class PatchAssemblyProvider : IPatchAssemblyProvider
{
    public Assembly PatchAssembly => Assembly.GetCallingAssembly();
}