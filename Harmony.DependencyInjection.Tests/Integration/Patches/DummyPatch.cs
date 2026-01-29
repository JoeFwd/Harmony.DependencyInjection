using System.Reflection;
using Harmony.DependencyInjection.Patches;
using HarmonyLib;

namespace Harmony.DependencyInjection.Tests.Integration.Patches;

public sealed class DummyPatch : IPatch
{
    public MethodInfo TargetMethod { get => typeof(DummyTarget).GetMethod(nameof(DummyTarget.GetValue)); }

    public MethodInfo PatchMethod { get => typeof(DummyPatch).GetMethod(nameof(Prefix)); }

    public PatchType PatchType
    {
        get => PatchType.Prefix;
    }

    public static bool Prefix(ref string __result)
    {
        __result = "Patched";
        return false;
    }
}