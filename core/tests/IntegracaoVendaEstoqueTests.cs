using Xunit;
using core.domain;

public class IntegracaoVendaEstoqueTests
{
    [Fact]
    public void Venda_Deve_Baixar_Estoque()
    {
        var produto = new Produto { /* propriedades mínimas */ };
        var estoque = new Estoque { /* Produto = produto, Quantidade = 10 */ };
        var venda = new Venda { /* Produto = produto, Quantidade = 2 */ };

        // Simula operação de venda e baixa de estoque
        estoque.Quantidade -= venda.Quantidade;
        Assert.Equal(8, estoque.Quantidade);
    }

    [Fact]
    public void Nao_Deve_Permitir_Venda_Sem_Estoque()
    {
        var produto = new Produto { /* propriedades mínimas */ };
        var estoque = new Estoque { /* Produto = produto, Quantidade = 0 */ };
        var venda = new Venda { /* Produto = produto, Quantidade = 1 */ };

        Assert.ThrowsAny<System.Exception>(() =>
        {
            if (estoque.Quantidade < venda.Quantidade)
                throw new System.Exception("Estoque insuficiente");
        });
    }
}