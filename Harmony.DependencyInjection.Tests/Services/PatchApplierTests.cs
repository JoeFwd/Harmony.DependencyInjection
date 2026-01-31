using System;
using System.Collections.Generic;
using System.Reflection;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Harmony.DependencyInjection.Tests.Services;

public class PatchApplierTests : TestBase
{
    public class DummyTarget
    {
        public void TargetMethod() { }
    }

    private static class DummyPatchMethods
    {
        // Simple static prefix method required by Harmony; matches target method signature
        public static void Prefix(DummyTarget __instance) { }
    }

    private class DummyPatch : IPatch
    {
        private MethodInfo? _targetMethod;
        private MethodInfo? _patchMethod;
        private PatchType _patchType;

        public DummyPatch(MethodInfo? targetMethod, MethodInfo? patchMethod, PatchType patchType)
        {
            _targetMethod = targetMethod;
            _patchMethod = patchMethod;
            _patchType = patchType;
        }

        public MethodInfo? TargetMethod => _targetMethod;
        public MethodInfo? PatchMethod => _patchMethod;
        public PatchType PatchType => _patchType;
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PatchApplier(null!));
    }

    [Fact]
    public void Apply_NullPatches_ThrowsArgumentNullException()
    {
        var loggerMock = GetLoggerMock<PatchApplier>();
        var applier = new PatchApplier(loggerMock.Object);
        var harmony = new HarmonyLib.Harmony("test");

        Assert.Throws<ArgumentNullException>(() => applier.Apply(null!, harmony));
    }

    [Fact]
    public void Apply_NullHarmony_ThrowsArgumentNullException()
    {
        var loggerMock = GetLoggerMock<PatchApplier>();
        var applier = new PatchApplier(loggerMock.Object);
        var patches = new List<IPatch>();

        Assert.Throws<ArgumentNullException>(() => applier.Apply(patches, null!));
    }

    [Fact]
    public void Apply_ValidPatches_AppliesAndLogsInformation()
    {
        var loggerMock = GetLoggerMock<PatchApplier>();
        var applier = new PatchApplier(loggerMock.Object);
        var harmonyMock = new Mock<HarmonyLib.Harmony>("test");
        var harmony = harmonyMock.Object;

        // dummy instance not needed for method reflection
        var targetMethod = typeof(DummyTarget).GetMethod(nameof(DummyTarget.TargetMethod))!;
        var patchMethod = typeof(DummyPatchMethods).GetMethod(nameof(DummyPatchMethods.Prefix))!; // use valid static prefix method

        var patch = new DummyPatch(targetMethod, patchMethod, PatchType.Prefix);

        var patches = new List<IPatch> { patch };

        applier.Apply(patches, harmony);

        // Verify that an information log was written for the applied patch
        loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            null,
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public void Apply_PatchWithNullTarget_LogsWarningAndContinues()
    {
        var loggerMock = GetLoggerMock<PatchApplier>();
        var applier = new PatchApplier(loggerMock.Object);
        var harmonyMock = new Mock<HarmonyLib.Harmony>("test");
        var harmony = harmonyMock.Object;

        var dummy = new DummyTarget();
        var targetMethod = (MethodInfo?)null; // null target
        var patchMethod = typeof(DummyTarget).GetMethod(nameof(DummyTarget.TargetMethod))!;

        var patch = new DummyPatch(targetMethod, patchMethod, PatchType.Prefix);
        var patches = new List<IPatch> { patch };

        applier.Apply(patches, harmony);

        // Verify warning logged
        loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            null,
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Apply_PatchWithNullPatchMethod_LogsWarningAndContinues()
    {
        var loggerMock = GetLoggerMock<PatchApplier>();
        var applier = new PatchApplier(loggerMock.Object);
        var harmonyMock = new Mock<HarmonyLib.Harmony>("test");
        var harmony = harmonyMock.Object;

        var dummy = new DummyTarget();
        var targetMethod = typeof(DummyTarget).GetMethod(nameof(DummyTarget.TargetMethod))!;
        var patchMethod = (MethodInfo?)null; // null patch method

        var patch = new DummyPatch(targetMethod, patchMethod, PatchType.Prefix);
        var patches = new List<IPatch> { patch };

        applier.Apply(patches, harmony);

        // Verify warning logged
        loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            null,
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Apply_PatchWithUnknownPatchType_LogsErrorAndContinues()
    {
        var loggerMock = GetLoggerMock<PatchApplier>();
        var applier = new PatchApplier(loggerMock.Object);
        var harmonyMock = new Mock<HarmonyLib.Harmony>("test");
        var harmony = harmonyMock.Object;

        var dummy = new DummyTarget();
        var targetMethod = typeof(DummyTarget).GetMethod(nameof(DummyTarget.TargetMethod))!;
        var patchMethod = typeof(DummyTarget).GetMethod(nameof(DummyTarget.TargetMethod))!;
        var unknownType = (PatchType)999; // invalid enum value

        var patch = new DummyPatch(targetMethod, patchMethod, unknownType);
        var patches = new List<IPatch> { patch };

        applier.Apply(patches, harmony);

        // Verify error logged
        loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            null,
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }
}
