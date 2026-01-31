using Microsoft.Extensions.Logging;
using Moq;

namespace Harmony.DependencyInjection.Tests.Services;

public abstract class TestBase
{
    protected Mock<ILogger<T>> GetLoggerMock<T>() where T : class
    {
        return new Mock<ILogger<T>>();
    }
}
