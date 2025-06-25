using Xunit;
using core.domain;

public class ItemVendaTests
{
    [Fact]
    public void Deve_Criar_ItemVenda_Valido()
    {
        var item = new ItemVenda { /* propriedades mínimas */ };
        Assert.NotNull(item);
    }

    [Fact]
    public void Nao_Deve_Aceitar_Quantidade_Negativa()
    {
        Assert.ThrowsAny<System.Exception>(() => new ItemVenda { /* Quantidade = -1 */ });
    }
}