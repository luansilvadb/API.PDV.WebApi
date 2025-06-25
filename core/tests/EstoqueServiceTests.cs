using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using API.PDV.Application;
using API.PDV.Domain;

public class EstoqueServiceTests
{
    [Fact]
    public async Task SaidaAsync_DeveReduzirEstoque_QuandoQuantidadeSuficiente()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var estoque = new Estoque { ProdutoId = produtoId, Quantidade = 10 };

        var repo = new Mock<IEstoqueRepository>();
        repo.Setup(r => r.GetByProdutoIdAsync(produtoId, null)).ReturnsAsync(estoque);
        repo.Setup(r => r.UpdateAsync(It.IsAny<Estoque>())).Returns(Task.CompletedTask);

        var service = new EstoqueService(repo.Object);

        // Act
        await service.SaidaAsync(produtoId, 5);

        // Assert
        Assert.Equal(5, estoque.Quantidade);
        repo.Verify(r => r.UpdateAsync(It.Is<Estoque>(e => e.Quantidade == 5)), Times.Once);
    }

    [Fact]
    public async Task SaidaAsync_DeveLancarExcecao_QuandoEstoqueInsuficiente()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var estoque = new Estoque { ProdutoId = produtoId, Quantidade = 3 };

        var repo = new Mock<IEstoqueRepository>();
        repo.Setup(r => r.GetByProdutoIdAsync(produtoId, null)).ReturnsAsync(estoque);

        var service = new EstoqueService(repo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SaidaAsync(produtoId, 5));
    }
}