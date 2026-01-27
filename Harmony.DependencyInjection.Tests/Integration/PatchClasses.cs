using System;
using System.Reflection;
using Harmony.DependencyInjection.Patches;

namespace Harmony.DependencyInjection.Tests.Integration
{
    public class TargetPatch : IPatch
    {
        public PatchType PatchType => PatchType.Prefix;

        public MethodInfo? TargetMethod => typeof(Target).GetMethod(nameof(Target.GetValue));

        public MethodInfo? PatchMethod => typeof(TargetPatch).GetMethod(nameof(PrefixMethod));

        public static bool PrefixMethod(ref int __result) { __result = 42; return false; }
    }

    public class TargetPatchTwo : IPatch
    {
        public PatchType PatchType => PatchType.Prefix;

        public MethodInfo? TargetMethod => typeof(Target).GetMethod(nameof(Target.GetValue));

        public MethodInfo? PatchMethod => typeof(TargetPatchTwo).GetMethod(nameof(PrefixMethod));

        public void Register(Microsoft.Extensions.DependencyInjection.IServiceCollection services) { }

        public static bool PrefixMethod(ref int __result) { __result = 10; return false; }
    }

    public class ConsumerPatch : IPatch
    {
        public PatchType PatchType => PatchType.Prefix;

        public MethodInfo? TargetMethod => typeof(Consumer).GetMethod(nameof(Consumer.GetConsumerValue));

        public MethodInfo? PatchMethod => typeof(ConsumerPatch).GetMethod(nameof(PrefixMethod));

        public void Register(Microsoft.Extensions.DependencyInjection.IServiceCollection services) { }

        public static bool PrefixMethod(ref int __result) { __result = 99; return false; }
    }

    // Dummy classes to match the dynamic assembly types
    public interface ITarget
    {
        int GetValue();
    }

    public class Target : ITarget
    {
        public int GetValue() => 1;
    }

    public class Consumer
    {
        private readonly ITarget _target;
        public Consumer(ITarget target) { _target = target; }
        public int GetConsumerValue() => _target.GetValue() + 1;
    }
}
