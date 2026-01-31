using System.Threading.Tasks;
using Harmony.DependencyInjection.Patches;
using Harmony.DependencyInjection.Tests.Integration.Patches;
using Harmony.DependencyInjection.Tests.Integration.Util;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Harmony.DependencyInjection.Tests.Integration;

public class IntegrationTests
{
    [Fact]
    public async Task Can_generate_compile_and_load_type()
    {
        // Arrange
        var source = $@"
            using Microsoft.Extensions.DependencyInjection;
            using System.Threading.Tasks;

            public class TestContainerFactory {{
                public async Task RunHostAsync()
                {{
                    var host = new {typeof(HostBuilder).FullName}()
                        .ConfigureServices((context, services) =>
                        {{
                            services.AddTransient<{typeof(IPatch).FullName}, {typeof(DummyPatch).FullName}>();
                            {typeof(HarmonyPatcherRegistration).FullName}.AddHarmonyPatching(services);
                        }})
                        .Build();

                    await host.StartAsync();
                }}
            }}";
        var assembly = RoslynCompilationService.Compile("DynamicAsm", source);

        // Act
        var factory = AssemblyLoader.CreateInstance(assembly, "TestContainerFactory");
        await (factory.GetType().GetMethod("RunHostAsync").Invoke(factory, null) as Task);

        // Assert
        var dummyTarget =
            AssemblyLoader.CreateInstance(typeof(DummyTarget).Assembly, typeof(DummyTarget).FullName) as DummyTarget;
        Assert.Equal("Patched", dummyTarget.GetValue());
    }
}