using System.Runtime.CompilerServices;

namespace Harmony.DependencyInjection.Tests.Integration.Patches;

public class DummyTarget
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public virtual string GetValue()
    {
        return "Original";
    }
}