using Microsoft.Extensions.Logging;
using Moq;

namespace Harmony.DependencyInjection.Tests;

public abstract class TestBase
{
    protected Mock<ILogger<T>> CreateLogger<T>() where T : class
    {
        return new Mock<ILogger<T>>();
    }
}
