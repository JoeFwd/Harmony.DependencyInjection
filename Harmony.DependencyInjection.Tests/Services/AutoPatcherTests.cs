using System;
using HarmonyLib;
using Harmony.DependencyInjection.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Harmony.DependencyInjection.Tests
{
    public class AutoPatcherTests : TestBase
    {
        [Fact]
        public void PatchAllLoadedAssemblies_DoesNotThrow()
        {
            // Arrange
            var loggerMock = CreateLogger<AutoPatcher>();
            var autoPatcher = new AutoPatcher(loggerMock.Object);
            var harmony = new HarmonyLib.Harmony("test.auto" + Guid.NewGuid());

            // Act & Assert: should not throw any exception.
            var exception = Record.Exception(() => autoPatcher.PatchAllLoadedAssemblies(harmony));
            Assert.Null(exception);
        }

        [Fact]
        public void PatchAllLoadedAssemblies_NullHarmony_ThrowsArgumentNullException()
        {
            var loggerMock = CreateLogger<AutoPatcher>();
            var autoPatcher = new AutoPatcher(loggerMock.Object);
            Assert.Throws<ArgumentNullException>(() => autoPatcher.PatchAllLoadedAssemblies(null!));
        }
    }
}
