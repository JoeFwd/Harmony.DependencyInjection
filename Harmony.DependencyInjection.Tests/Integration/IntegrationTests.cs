using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Tests.Integration;

namespace Harmony.DependencyInjection.Tests
{
    public class IntegrationTests : TestBase
    {
        [Fact]
        public void Patch_WithSingleDependency_AppliesCorrectly()
        {
            var assembly = typeof(TargetPatch).Assembly;
            var iTargetType = assembly.GetType("Harmony.DependencyInjection.Tests.Integration.ITarget");
            var targetType = assembly.GetType("Harmony.DependencyInjection.Tests.Integration.Target");
            var targetPatchType = assembly.GetType("Harmony.DependencyInjection.Tests.Integration.TargetPatch");

            
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddTransient(iTargetType, targetType);
            services.AddTransient(typeof(IPatch), targetPatchType);

            services.AddHarmonyPatching();

            var provider = services.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true });

            var patcher = provider.GetRequiredService<HarmonyPatcher>();
            patcher.Apply();

            var targetInstance = (dynamic)provider.GetRequiredService(iTargetType);
            int result = targetInstance.GetValue();

            Assert.Equal(42, result);
        }

//         [Fact]
//         public void Patch_WithTwoDependencies_AppliesBothCorrectly()
//         {
//             var code = @"
// using System;
// using Harmony.DependencyInjection.Patches;
// namespace DynamicNamespace
// {
//     public interface ITarget { int GetValue(); }
//     public class Target : ITarget { public int GetValue() => 1; }
//     public class Consumer
//     {
//         private readonly ITarget _target;
//         public Consumer(ITarget target) { _target = target; }
//         public int GetConsumerValue() => _target.GetValue() + 1;
//     }
//     public class TargetPatch : IPatch
//     {
//         public PatchType PatchType => PatchType.Prefix;
//         public Type TargetType => typeof(Target);
//         public string TargetMethodName => nameof(Target.GetValue);
//         public MethodInfo Prefix => typeof(TargetPatch).GetMethod(nameof(PrefixMethod));
//         public static bool PrefixMethod(ref int __result) { __result = 10; return false; }
//     }
//     public class ConsumerPatch : IPatch
//     {
//         public PatchType PatchType => PatchType.Prefix;
//         public Type TargetType => typeof(Consumer);
//         public string TargetMethodName => nameof(Consumer.GetConsumerValue);
//         public MethodInfo Prefix => typeof(ConsumerPatch).GetMethod(nameof(PrefixMethod));
//         public static bool PrefixMethod(ref int __result) { __result = 99; return false; }
//     }
// }
// ";
//             var assembly = typeof(TargetPatch).Assembly;
//             var iTarget = assembly.GetType("Harmony.DependencyInjection.Tests.Integration.ITarget");
//             var target = assembly.GetType("Harmony.DependencyInjection.Tests.Integration.Target");
//             var consumer = assembly.GetType("Harmony.DependencyInjection.Tests.Integration.Consumer");
//             var targetPatch = assembly.GetType("Harmony.DependencyInjection.Tests.Integration.TargetPatchTwo");
//             var consumerPatch = assembly.GetType("Harmony.DependencyInjection.Tests.Integration.ConsumerPatch");
//
//             var services = new ServiceCollection();
//             services.AddTransient(iTarget, target);
//             services.AddTransient(consumer);
//             services.AddTransient(typeof(IPatch), targetPatch);
//             services.AddTransient(typeof(IPatch), consumerPatch);
//             var provider = services.BuildServiceProvider();
//
//             var harmony = new HarmonyLib.Harmony("test.two" + Guid.NewGuid());
//             var patcher = new HarmonyPatcher(provider, harmony);
//             patcher.Apply();
//
//             var consumerInstance = (dynamic)provider.GetRequiredService(consumer);
//             int consumerResult = consumerInstance.GetConsumerValue();
//             Assert.Equal(99, consumerResult);
//
//             var targetInstance = (dynamic)provider.GetRequiredService(iTarget);
//             int targetResult = targetInstance.GetValue();
//             Assert.Equal(10, targetResult);
//         }
    }
}
