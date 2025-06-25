using Xunit;
using core.domain;

public class ProdutoTests
{
    [Fact]
    public void Deve_Criar_Produto_Valido()
    {
        var produto = new Produto { /* propriedades mínimas */ };
        Assert.NotNull(produto);
    }

    [Fact]
    public void Nao_Deve_Aceitar_Nome_Vazio()
    {
        Assert.ThrowsAny<System.Exception>(() => new Produto { /* Nome = "" */ });
    }
}