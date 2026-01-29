using System.Reflection;

namespace Harmony.DependencyInjection.Services;

public interface IPatchAssemblyProvider
{
    Assembly PatchAssembly { get; }
}