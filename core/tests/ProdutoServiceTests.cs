using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.PDV.Domain;
using API.PDV.Application;
using Moq;
using Xunit;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _repoMock;
    private readonly ProdutoService _service;

    public ProdutoServiceTests()
    {
        _repoMock = new Mock<IProdutoRepository>();
        _service = new ProdutoService(_repoMock.Object);
    }

    [Fact]
    public async Task CriarAsync_Deve_Retornar_Produto()
    {
        var produto = new Produto();
        _repoMock.Setup(r => r.AddAsync(produto)).ReturnsAsync(produto);

        var result = await _service.CriarAsync(produto);

        Assert.Equal(produto, result);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Produto_Quando_Encontrado()
    {
        var produto = new Produto();
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(produto);

        var result = await _service.ObterPorIdAsync(Guid.NewGuid());

        Assert.Equal(produto, result);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto)null);

        var result = await _service.ObterPorIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task ListarAsync_Deve_Retornar_Lista()
    {
        var lista = new List<Produto> { new Produto() };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(lista);

        var result = await _service.ListarAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Chamar_Repositorio()
    {
        var produto = new Produto();
        _repoMock.Setup(r => r.UpdateAsync(produto)).Returns(Task.CompletedTask);

        await _service.AtualizarAsync(produto);

        _repoMock.Verify(r => r.UpdateAsync(produto), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_Deve_Chamar_Repositorio()
    {
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

        await _service.RemoverAsync(id);

        _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Deve_Lancar_Excecao_Se_Repositorio_Falhar()
    {
        var produto = new Produto();
        _repoMock.Setup(r => r.AddAsync(produto)).ThrowsAsync(new Exception("Erro"));

        await Assert.ThrowsAsync<Exception>(() => _service.CriarAsync(produto));
    }
}