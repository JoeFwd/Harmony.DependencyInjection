using System;
using System.Collections.Generic;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Harmony.DependencyInjection.Tests.Services;

public class PatchDiscoveryTests : TestBase
{
    [Fact]
    public void Discover_ReturnsAllPatches_AndLogsInformation()
    {
        // Arrange
        Mock<ILogger<PatchDiscovery>> loggerMock = GetLoggerMock<PatchDiscovery>();
        Mock<IPatch> patchMock1 = new();
        Mock<IPatch> patchMock2 = new();
        List<IPatch> patches = new() { patchMock1.Object, patchMock2.Object };
        var discovery = new PatchDiscovery(patches, loggerMock.Object);

        // Act
        IReadOnlyList<IPatch> result = discovery.Discover();

        // Assert
        Assert.Equal(patches, result);
        loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Exactly(patches.Count));
    }
}