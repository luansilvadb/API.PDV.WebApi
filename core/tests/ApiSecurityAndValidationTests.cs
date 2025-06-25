using System;
using API.PDV.Domain;
using API.PDV.Application;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class ApiSecurityAndValidationTests
{
    [Fact(DisplayName = "CpfCnpj deve persistir valor criptografado e retornar valor real")]
    public void CpfCnpj_Criptografia_DeveFuncionar()
    {
        var valor = "12345678901";
        var cpf = new CpfCnpj(valor);
        var encrypted = cpf.GetEncrypted();

        Assert.NotEqual(valor, encrypted);
        Assert.Equal(valor, cpf.Valor);

        // Testa construtor decriptando
        var cpf2 = new CpfCnpj(encrypted, true);
        Assert.Equal(valor, cpf2.Valor);
    }

    [Fact(DisplayName = "LoggingService deve mascarar CPF/CNPJ nos logs")]
    public void LoggingService_Deve_Mascarar_CpfCnpj()
    {
        var loggerMock = new Mock<ILogger<LoggingService>>();
        var service = new LoggingService(loggerMock.Object);

        string msg = "Cliente CPF 123.456.789-01 fez operação. CNPJ: 12.345.678/0001-99";
        service.LogInfo(msg);

        loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) =>
                !v.ToString().Contains("123.456.789-01") &&
                !v.ToString().Contains("12.345.678/0001-99") &&
                v.ToString().Contains("***")
            ),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
}