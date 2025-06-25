using System;
using core.application;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class LoggingServiceTests
{
    [Fact]
    public void LogInfo_Deve_Chamar_LogInformation()
    {
        var loggerMock = new Mock<ILogger<LoggingService>>();
        var service = new LoggingService(loggerMock.Object);

        service.LogInfo("mensagem");

        loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("mensagem")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public void LogError_Deve_Chamar_LogError()
    {
        var loggerMock = new Mock<ILogger<LoggingService>>();
        var service = new LoggingService(loggerMock.Object);
        var ex = new Exception("erro");

        service.LogError("msg", ex);

        loggerMock.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("msg")),
            ex,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
}