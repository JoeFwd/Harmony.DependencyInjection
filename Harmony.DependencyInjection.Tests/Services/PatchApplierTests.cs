// Unit tests for PatchApplier service

using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Harmony.DependencyInjection.Tests.Services
{
    // Target class whose method will be patched.
    public class TargetClass
    {
        public static bool WasPatched = false;
        public static void TargetMethod() { }
    }

    // Prefix patch that sets WasPatched flag.
    public class PrefixPatch : IPatch
    {
        public MethodInfo? TargetMethod => typeof(TargetClass).GetMethod(nameof(TargetClass.TargetMethod));
        public MethodInfo? PatchMethod => typeof(PrefixPatch).GetMethod(nameof(Prefix));
        public PatchType PatchType => PatchType.Prefix;
        public static void Prefix() => TargetClass.WasPatched = true;
    }

    public class PatchApplierTests : TestBase
    {
        [Fact]
        public void Apply_PrefixPatch_SetsFlag()
        {
            // Arrange
            var loggerMock = CreateLogger<PatchApplier>();
            var applier = new PatchApplier(loggerMock.Object);
            var harmony = new HarmonyLib.Harmony("test.harmony" + Guid.NewGuid());
            var patch = new PrefixPatch();

            // Act
            applier.Apply(new[] { patch }, harmony);
            // Invoke the target method after patching.
            TargetClass.TargetMethod();

            // Assert
            Assert.True(TargetClass.WasPatched, "Prefix patch should have set WasPatched flag.");
        }

        [Fact]
        public void Apply_ShouldThrowArgumentNullException_WhenPatchesNull()
        {
            // Arrange
            var loggerMock = CreateLogger<PatchApplier>();
            var applier = new PatchApplier(loggerMock.Object);
            var harmony = new HarmonyLib.Harmony("test.harmony" + Guid.NewGuid());

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => applier.Apply(null, harmony));
        }

        // Additional edge case tests for PatchApplier
        [Fact]
        public void Constructor_NullLogger_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new PatchApplier(null!));
        }

        [Fact]
        public void Apply_EmptyCollection_DoesNotThrow()
        {
            var logger = CreateLogger<PatchApplier>();
            var applier = new PatchApplier(logger.Object);
            var harmony = new HarmonyLib.Harmony("test.empty" + Guid.NewGuid());
            var empty = new List<IPatch>();
            var ex = Record.Exception(() => applier.Apply(empty, harmony));
            Assert.Null(ex);
        }

        private class SimpleTarget
        {
            public static bool WasPatched = false;
            public static void TargetMethod() { }
        }

        private class SimplePrefixPatch : IPatch
        {
            public void Register(IServiceCollection services) { }
            public MethodInfo? TargetMethod => typeof(SimpleTarget).GetMethod(nameof(SimpleTarget.TargetMethod));
            public MethodInfo? PatchMethod => typeof(SimplePrefixPatch).GetMethod(nameof(Prefix));
            public PatchType PatchType => PatchType.Prefix;
            public static void Prefix() => SimpleTarget.WasPatched = true;
        }

        [Fact]
        public void Apply_DuplicatePatches_NoException()
        {
            var logger = CreateLogger<PatchApplier>();
            var applier = new PatchApplier(logger.Object);
            var harmony = new HarmonyLib.Harmony("test.dup" + Guid.NewGuid());
            var patches = new List<IPatch> { new SimplePrefixPatch(), new SimplePrefixPatch() };
            var ex = Record.Exception(() => applier.Apply(patches, harmony));
            Assert.Null(ex);
            SimpleTarget.WasPatched = false;
            SimpleTarget.TargetMethod();
            Assert.True(SimpleTarget.WasPatched);
        }

        private class UnknownPatchType : IPatch
        {
            public void Register(IServiceCollection services) { }
            public MethodInfo? TargetMethod => typeof(SimpleTarget).GetMethod(nameof(SimpleTarget.TargetMethod));
            public MethodInfo? PatchMethod => typeof(UnknownPatchType).GetMethod(nameof(Prefix));
            public PatchType PatchType => (PatchType)999; // invalid
            public static void Prefix() => SimpleTarget.WasPatched = true;
        }

        [Fact]
        public void Apply_UnsupportedPatchType_NoException()
        {
            var logger = CreateLogger<PatchApplier>();
            var applier = new PatchApplier(logger.Object);
            var harmony = new HarmonyLib.Harmony("test.unknown" + Guid.NewGuid());
            var patches = new List<IPatch> { new UnknownPatchType() };
            var ex = Record.Exception(() => applier.Apply(patches, harmony));
            Assert.Null(ex);
        }

        private class ThrowingPatch : IPatch
        {
            public void Register(IServiceCollection services) { }
            public MethodInfo? TargetMethod => typeof(SimpleTarget).GetMethod(nameof(SimpleTarget.TargetMethod));
            public MethodInfo? PatchMethod => typeof(ThrowingPatch).GetMethod(nameof(Throw));
            public PatchType PatchType => PatchType.Prefix;
            public static void Throw()
            {
                throw new InvalidOperationException("failure");
            }
        }

        [Fact]
        public void Apply_PatchMethodThrows_NoException()
        {
            var logger = CreateLogger<PatchApplier>();
            var applier = new PatchApplier(logger.Object);
            var harmony = new HarmonyLib.Harmony("test.throw" + Guid.NewGuid());
            var patches = new List<IPatch> { new ThrowingPatch() };
            var ex = Record.Exception(() => applier.Apply(patches, harmony));
            Assert.Null(ex);
        }

        private class MissingMethodsPatch : IPatch
        {
            public MethodInfo? TargetMethod => null;
            public MethodInfo? PatchMethod => null;
            public PatchType PatchType => PatchType.Prefix;
        }

        [Fact]
        public void Apply_MissingMethods_NoException()
        {
            var logger = CreateLogger<PatchApplier>();
            var applier = new PatchApplier(logger.Object);
            var harmony = new HarmonyLib.Harmony("test.missing" + Guid.NewGuid());
            var patches = new List<IPatch> { new MissingMethodsPatch() };
            var ex = Record.Exception(() => applier.Apply(patches, harmony));
            Assert.Null(ex);
        }
    }
}
