using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using API.PDV.Application;
using API.PDV.Domain;
using Microsoft.Extensions.Logging;

namespace API.PDV.Tests
{
    public class RelatorioServiceTests
    {
        private readonly Mock<IRelatorioRepository> _repoMock;
        private readonly Mock<ILogger<LoggingService>> _loggerMock;
        private readonly LoggingService _loggingService;
        private readonly RelatorioService _service;

        public RelatorioServiceTests()
        {
            _repoMock = new Mock<IRelatorioRepository>();
            _loggerMock = new Mock<ILogger<LoggingService>>();
            _loggingService = new LoggingService(_loggerMock.Object);
            _service = new RelatorioService(_repoMock.Object, _loggingService);
        }

        [Fact]
        public async Task ObterVendasPorPeriodoAsync_DeveRetornarVendas()
        {
            var inicio = DateTime.Today.AddDays(-7);
            var fim = DateTime.Today;
            var vendas = new List<VendaResumoDto>
            {
                new VendaResumoDto { Id = Guid.NewGuid(), DataVenda = inicio, Total = 100, Status = "Fechada" }
            };
            _repoMock.Setup(r => r.ObterVendasPorPeriodoAsync(inicio, fim)).ReturnsAsync(vendas);

            var result = await _service.ObterVendasPorPeriodoAsync(inicio, fim);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ObterEstoqueAtualAsync_DeveRetornarEstoque()
        {
            var estoque = new List<EstoqueAtualDto>
            {
                new EstoqueAtualDto { ProdutoId = Guid.NewGuid(), Nome = "Produto X", Quantidade = 10, Lote = "A1" }
            };
            _repoMock.Setup(r => r.ObterEstoqueAtualAsync()).ReturnsAsync(estoque);

            var result = await _service.ObterEstoqueAtualAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ObterProdutosMaisVendidosAsync_DeveRetornarProdutos()
        {
            var inicio = DateTime.Today.AddDays(-30);
            var fim = DateTime.Today;
            var produtos = new List<ProdutoMaisVendidoDto>
            {
                new ProdutoMaisVendidoDto { ProdutoId = Guid.NewGuid(), Nome = "Produto Y", QuantidadeVendida = 50 }
            };
            _repoMock.Setup(r => r.ObterProdutosMaisVendidosAsync(inicio, fim, 10)).ReturnsAsync(produtos);

            var result = await _service.ObterProdutosMaisVendidosAsync(inicio, fim, 10);

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ObterVendasPorPeriodoAsync_FalhaDeInfraestrutura_DeveRetornarListaVaziaEFazerLog()
        {
            var inicio = DateTime.Today.AddDays(-7);
            var fim = DateTime.Today;
            _repoMock.SetupSequence(r => r.ObterVendasPorPeriodoAsync(inicio, fim))
                .ThrowsAsync(new Exception("Falha infra"))
                .ThrowsAsync(new Exception("Falha infra"))
                .ReturnsAsync(new List<VendaResumoDto>());

            var result = await _service.ObterVendasPorPeriodoAsync(inicio, fim);

            Assert.NotNull(result);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Falha ao obter relatório de vendas por período.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task ObterEstoqueAtualAsync_FalhaDeInfraestrutura_DeveRetornarListaVaziaEFazerLog()
        {
            _repoMock.Setup(r => r.ObterEstoqueAtualAsync()).ThrowsAsync(new Exception("Falha infra"));

            var result = await _service.ObterEstoqueAtualAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Falha ao obter relatório de estoque atual.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task ObterProdutosMaisVendidosAsync_FalhaDeInfraestrutura_DeveRetornarListaVaziaEFazerLog()
        {
            var inicio = DateTime.Today.AddDays(-30);
            var fim = DateTime.Today;
            _repoMock.Setup(r => r.ObterProdutosMaisVendidosAsync(inicio, fim, 10)).ThrowsAsync(new Exception("Falha infra"));

            var result = await _service.ObterProdutosMaisVendidosAsync(inicio, fim, 10);

            Assert.NotNull(result);
            Assert.Empty(result);
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Falha ao obter relatório de produtos mais vendidos.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }
    }
}