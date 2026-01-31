using System.Reflection;

namespace Harmony.DependencyInjection.Services;

internal interface IPatchAssemblyProvider
{
    Assembly PatchAssembly { get; }
}