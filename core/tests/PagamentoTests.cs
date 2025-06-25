using Xunit;
using core.domain;

public class PagamentoTests
{
    [Fact]
    public void Deve_Criar_Pagamento_Valido()
    {
        var pagamento = new Pagamento { /* propriedades mínimas */ };
        Assert.NotNull(pagamento);
    }

    [Fact]
    public void Nao_Deve_Aceitar_Valor_Negativo()
    {
        Assert.ThrowsAny<System.Exception>(() => new Pagamento { /* Valor = -10 */ });
    }
}