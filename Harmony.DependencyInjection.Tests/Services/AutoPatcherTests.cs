using System;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Harmony.DependencyInjection.Tests.Services;

public class AutoPatcherTests : TestBase
{
    [Fact(Skip = "Skipped for patch applier test suite")]
    public void PatchAllLoadedAssemblies_CallsHarmonyPatchAll_AndLogsInformation()
    {
        // Arrange
        var loggerMock = GetLoggerMock<AutoPatcher>();
        var assemblyProviderMock = new Mock<IPatchAssemblyProvider>();
        var dummyAssembly = typeof(AutoPatcher).Assembly; // using the current assembly for simplicity
        assemblyProviderMock.Setup(p => p.PatchAssembly).Returns(dummyAssembly);

        var harmony = new HarmonyLib.Harmony("test");
        var autoPatcher = new AutoPatcher(loggerMock.Object, assemblyProviderMock.Object);

        // Act
        autoPatcher.PatchAllLoadedAssemblies(harmony);

        // Assert
        // Verify that logger recorded information about the patching process
        loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            null,
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }

    [Fact(Skip = "Skipped for patch applier test suite")]
    public void PatchAllLoadedAssemblies_NullHarmony_ThrowsArgumentNullException()
    {
        // Arrange
        var loggerMock = GetLoggerMock<AutoPatcher>();
        var assemblyProviderMock = new Mock<IPatchAssemblyProvider>();
        assemblyProviderMock.Setup(p => p.PatchAssembly).Returns(typeof(AutoPatcher).Assembly);
        var autoPatcher = new AutoPatcher(loggerMock.Object, assemblyProviderMock.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => autoPatcher.PatchAllLoadedAssemblies(null!));
    }
}
