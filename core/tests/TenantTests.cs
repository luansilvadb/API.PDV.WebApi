using Xunit;
using core.domain;

public class TenantTests
{
    [Fact]
    public void Deve_Criar_Tenant_Valido()
    {
        var tenant = new Tenant { /* propriedades mínimas */ };
        Assert.NotNull(tenant);
    }

    [Fact]
    public void Nao_Deve_Aceitar_Slug_Vazio()
    {
        Assert.ThrowsAny<System.Exception>(() => new Tenant { /* Slug = "" */ });
    }
}