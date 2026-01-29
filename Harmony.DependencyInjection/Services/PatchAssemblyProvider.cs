using System.Reflection;

namespace Harmony.DependencyInjection.Services;

public class PatchAssemblyProvider : IPatchAssemblyProvider
{
    public Assembly PatchAssembly { get => Assembly.GetCallingAssembly(); }
}