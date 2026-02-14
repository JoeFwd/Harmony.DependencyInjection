using Harmony.DependencyInjection.Tests.Integration.Patches;
using Harmony.DependencyInjection.Tests.Integration.Util;
using Xunit;

namespace Harmony.DependencyInjection.Tests.Integration;

public class IntegrationTests
{
    [Fact]
    public void Can_generate_compile_and_load_type()
    {
        // Arrange
        var source = @"
        using Microsoft.Extensions.DependencyInjection;
        using Harmony.DependencyInjection.Patches;
        using Harmony.DependencyInjection;
        using Harmony.DependencyInjection.Tests.Integration.Patches;
        using System;

        public class TestContainerFactory {

            public void Run()
            {
                IServiceCollection serviceCollection = new ServiceCollection();
                serviceCollection.AddSingleton<IPatch, DummyPatch>();
                serviceCollection.AddLogging();
                serviceCollection.AddHarmonyPatching();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                serviceProvider.GetService<IHarmonyPatcher>().ApplyPatches();
            }
        }";
        var assembly = RoslynCompilationService.Compile("DynamicAsm", source);
        var factory = AssemblyLoader.CreateInstance(assembly, "TestContainerFactory");

        // Act
        factory.GetType().GetMethod("Run").Invoke(factory, null);

        // Assert
        var dummyTarget =
            AssemblyLoader.CreateInstance(typeof(DummyTarget).Assembly, typeof(DummyTarget).FullName) as DummyTarget;
        Assert.Equal("Patched", dummyTarget.GetValue());
    }
}