using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Harmony.DependencyInjection.Patches
{
    public interface IPatch
    {
        MethodInfo? TargetMethod { get; }
        MethodInfo? PatchMethod { get; }
        PatchType PatchType { get; }
    }
}