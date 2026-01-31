using System.Reflection;
using Harmony.DependencyInjection.Services;
using Xunit;

namespace Harmony.DependencyInjection.Tests.Services;

public class PatchAssemblyProviderTests : TestBase
{
    [Fact]
    public void PatchAssembly_ReturnsCallingAssembly()
    {
        var provider = new PatchAssemblyProvider();
        Assembly assembly = provider.PatchAssembly;
        // The calling assembly should be the test assembly where this test runs.
        Assert.Equal(typeof(PatchAssemblyProviderTests).Assembly, assembly);
    }
}
