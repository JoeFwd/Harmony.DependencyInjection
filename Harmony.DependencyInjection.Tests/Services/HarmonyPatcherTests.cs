using System.Collections.Generic;
using System.Linq;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.Logging;
using System;
using Moq;
using Xunit;

namespace Harmony.DependencyInjection.Tests;

public class HarmonyPatcherTests : TestBase
{
    // [Fact]
    // public void Apply_Invokes_Discovery_Applier_AutoPatcher()
    // {
    //     var logger = CreateLogger<HarmonyPatcher>();
    //     var discoveryMock = new Mock<IPatchDiscovery>(MockBehavior.Strict);
    //     var applierMock = new Mock<IPatchApplier>(MockBehavior.Strict);
    //     var autoPatcherMock = new Mock<IAutoPatcher>(MockBehavior.Strict);
    //
    //     // Set up discovery to return empty list of patches.
    //     discoveryMock.Setup(d => d.Discover()).Returns(new List<IPatch>()).Verifiable();
    //
    //     // Capture arguments passed to Apply.
    //     IEnumerable<IPatch> capturedPatches = null;
    //     HarmonyLib.Harmony capturedHarmony = null;
    //     applierMock.Setup(a => a.Apply(It.IsAny<IEnumerable<IPatch>>(), It.Is<HarmonyLib.Harmony>(h => h != null)))
    //         .Callback<IEnumerable<IPatch>, HarmonyLib.Harmony>((p, h) => { capturedPatches = p; capturedHarmony = h; })
    //         .Verifiable();
    //
    //     // Capture argument passed to PatchAllLoadedAssemblies.
    //     HarmonyLib.Harmony capturedHarmonyPatch = null;
    //     autoPatcherMock.Setup(a => a.PatchAllLoadedAssemblies(It.Is<HarmonyLib.Harmony>(h => h != null)))
    //         .Callback<HarmonyLib.Harmony>(h => capturedHarmonyPatch = h)
    //         .Verifiable();
    //
    //     var patcher = new HarmonyPatcher(
    //         logger.Object,
    //         discoveryMock.Object,
    //         applierMock.Object,
    //         autoPatcherMock.Object);
    //
    //     patcher.Apply();
    //
    //     // Verify strict expectations.
    //     discoveryMock.Verify();
    //     applierMock.Verify();
    //     autoPatcherMock.Verify();
    //
    //     // Ensure the captured arguments are not null.
    //     Assert.NotNull(capturedPatches);
    //     Assert.NotNull(capturedHarmony);
    //     Assert.NotNull(capturedHarmonyPatch);
    // }
    //
    // [Fact]
    // public void Dispose_Calls_Remove()
    // {
    //     var logger = CreateLogger<HarmonyPatcher>();
    //     var discoveryMock = new Mock<IPatchDiscovery>(MockBehavior.Strict);
    //     var applierMock = new Mock<IPatchApplier>(MockBehavior.Strict);
    //     var autoPatcherMock = new Mock<IAutoPatcher>(MockBehavior.Strict);
    //
    //     discoveryMock.Setup(d => d.Discover()).Returns(new List<IPatch>()).Verifiable();
    //
    //     var patcher = new HarmonyPatcher(
    //         logger.Object,
    //         discoveryMock.Object,
    //         applierMock.Object,
    //         autoPatcherMock.Object);
    //
    //     // Ensure Dispose does not throw and calls Remove internally.
    //     patcher.Dispose();
    //     // Verify that Remove (which logs information) was called via Log method with Information level.
    //     logger.Verify(l => l.Log(
    //         LogLevel.Information,
    //         It.IsAny<EventId>(),
    //         It.IsAny<object>(),
    //         It.IsAny<Exception>(),
    //         (Func<object, Exception, string>)It.IsAny<object>()), Times.Once());
    // }
}
