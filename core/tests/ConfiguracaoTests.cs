using System;
using API.PDV.Domain;
using Xunit;

public class ConfiguracaoTests
{
    [Fact]
    public void Deve_Criar_Configuracao_Com_Valores_Validos()
    {
        var empresa = new Empresa();
        var config = new Configuracao
        {
            Id = Guid.NewGuid(),
            PermiteVendaFiado = true,
            DiasFechamentoCaixa = 7,
            Observacoes = "Teste",
            EmpresaId = Guid.NewGuid(),
            Empresa = empresa
        };

        Assert.True(config.PermiteVendaFiado);
        Assert.Equal(7, config.DiasFechamentoCaixa);
        Assert.Equal("Teste", config.Observacoes);
        Assert.NotNull(config.Empresa);
    }

    [Fact]
    public void Deve_Aceitar_Observacoes_Nulas()
    {
        var config = new Configuracao
        {
            Id = Guid.NewGuid(),
            PermiteVendaFiado = false,
            DiasFechamentoCaixa = 0,
            Observacoes = null,
            EmpresaId = Guid.NewGuid(),
            Empresa = null
        };

        Assert.Null(config.Observacoes);
        Assert.Null(config.Empresa);
    }
}