using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using API.PDV.Application;
using API.PDV.Domain;
using Microsoft.Extensions.Logging;

public class VendaServiceTests
{
    private readonly Mock<IVendaRepository> _vendaRepo;
    private readonly Mock<IItemVendaRepository> _itemVendaRepo;
    private readonly Mock<IPagamentoRepository> _pagamentoRepo;
    private readonly Mock<IEstoqueRepository> _estoqueRepo;
    private readonly Mock<IProdutoRepository> _produtoRepo;
    private readonly Mock<ILogger<LoggingService>> _loggerMock;
    private readonly LoggingService _loggingService;
    private readonly VendaService _service;

    public VendaServiceTests()
    {
        _vendaRepo = new Mock<IVendaRepository>();
        _itemVendaRepo = new Mock<IItemVendaRepository>();
        _pagamentoRepo = new Mock<IPagamentoRepository>();
        _estoqueRepo = new Mock<IEstoqueRepository>();
        _produtoRepo = new Mock<IProdutoRepository>();
        _loggerMock = new Mock<ILogger<LoggingService>>();
        _loggingService = new LoggingService(_loggerMock.Object);

        _service = new VendaService(
            _vendaRepo.Object,
            _itemVendaRepo.Object,
            _pagamentoRepo.Object,
            _estoqueRepo.Object,
            _produtoRepo.Object,
            _loggingService
        );
    }

    [Fact]
    public async Task FecharVendaAsync_DeveFinalizarVendaEPersistirTotal()
    {
        // Arrange
        var vendaId = Guid.NewGuid();
        var venda = new Venda { Id = vendaId, Finalizada = false };
        var itens = new List<ItemVenda>
        {
            new ItemVenda { Total = 10 },
            new ItemVenda { Total = 20 }
        };

        _vendaRepo.Setup(r => r.ObterPorIdAsync(vendaId)).ReturnsAsync(venda);
        _vendaRepo.Setup(r => r.AtualizarAsync(It.IsAny<Venda>())).Returns(Task.CompletedTask);

        _itemVendaRepo.Setup(r => r.ListarPorVendaAsync(vendaId)).ReturnsAsync(itens);

        // Act
        var result = await _service.FecharVendaAsync(vendaId);

        // Assert
        Assert.True(result.Finalizada);
        Assert.Equal(30, result.Total);
        _vendaRepo.Verify(r => r.AtualizarAsync(It.Is<Venda>(v => v.Finalizada && v.Total == 30)), Times.Once);
    }

    [Fact]
    public async Task FecharVendaAsync_FalhaDeInfraestrutura_DeveTentarRetryELogarEExcecaoCustomizada()
    {
        var vendaId = Guid.NewGuid();
        _vendaRepo.Setup(r => r.ObterPorIdAsync(vendaId)).ThrowsAsync(new Exception("Falha infra"));

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.FecharVendaAsync(vendaId));
        Assert.Contains("Erro ao fechar venda", ex.Message);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Falha ao fechar venda.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);

        _vendaRepo.Verify(r => r.ObterPorIdAsync(vendaId), Times.AtLeast(2)); // Retry
    }

    [Fact]
    public async Task IniciarVendaAsync_FalhaDeInfraestrutura_DeveTentarRetryELogarEExcecaoCustomizada()
    {
        _vendaRepo.Setup(r => r.AdicionarAsync(It.IsAny<Venda>())).ThrowsAsync(new Exception("Falha infra"));

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.IniciarVendaAsync(Guid.NewGuid()));
        Assert.Contains("Erro ao iniciar venda", ex.Message);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Falha ao iniciar venda.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);

        _vendaRepo.Verify(r => r.AdicionarAsync(It.IsAny<Venda>()), Times.AtLeast(2)); // Retry
    }

    [Fact]
    public async Task AdicionarItemAsync_FalhaDeInfraestrutura_DeveTentarRetryELogarEExcecaoCustomizada()
    {
        _produtoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Falha infra"));

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.AdicionarItemAsync(Guid.NewGuid(), Guid.NewGuid(), 1));
        Assert.Contains("Erro ao adicionar item à venda", ex.Message);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Falha ao adicionar item à venda.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);

        _produtoRepo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.AtLeast(2)); // Retry
    }

    [Fact]
    public async Task ReceberPagamentoAsync_FalhaDeInfraestrutura_DeveTentarRetryELogarEExcecaoCustomizada()
    {
        _vendaRepo.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Falha infra"));

        var ex = await Assert.ThrowsAsync<ApplicationException>(() => _service.ReceberPagamentoAsync(Guid.NewGuid(), 10, "Dinheiro"));
        Assert.Contains("Erro ao receber pagamento", ex.Message);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Falha ao receber pagamento.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);

        _vendaRepo.Verify(r => r.ObterPorIdAsync(It.IsAny<Guid>()), Times.AtLeast(2)); // Retry
    }
}