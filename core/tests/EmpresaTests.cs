using System;
using API.PDV.Domain;
using Xunit;

public class EmpresaTests
{
    private Empresa NovaEmpresa(NichoTipo nicho)
    {
        return new Empresa
        {
            Id = Guid.NewGuid(),
            Nome = "Teste",
            Documento = new CpfCnpj("12345678901"),
            Endereco = new Endereco("Rua", "1", "Cidade", "UF", "00000-000"),
            Nicho = nicho,
            Configuracao = new Configuracao(),
            TenantId = Guid.NewGuid()
        };
    }

    [Theory]
    [InlineData(NichoTipo.Mercado, 10, 2, 0, 20)]
    [InlineData(NichoTipo.Mercado, 10, 0, 3, 30)]
    [InlineData(NichoTipo.Distribuidora, 15, 0, 4, 60)]
    [InlineData(NichoTipo.Feira, 5, 2.5, 0, 12.5)]
    public void CalcularPrecoFinal_Deve_Retornar_Correto(NichoTipo nicho, decimal preco, decimal peso, int qtd, decimal esperado)
    {
        var empresa = NovaEmpresa(nicho);
        var resultado = empresa.CalcularPrecoFinal(preco, peso, qtd);
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void CalcularPrecoFinal_Feira_Sem_Peso_Deve_Lancar()
    {
        var empresa = NovaEmpresa(NichoTipo.Feira);
        Assert.Throws<InvalidOperationException>(() => empresa.CalcularPrecoFinal(10, 0, 2));
    }

    [Fact]
    public void ValidarOperacaoPorNicho_Mercado_Sem_Validade_Deve_Lancar()
    {
        var empresa = NovaEmpresa(NichoTipo.Mercado);
        Assert.Throws<InvalidOperationException>(() => empresa.ValidarOperacaoPorNicho(false, false));
    }

    [Fact]
    public void ValidarOperacaoPorNicho_Distribuidora_VendaPorPeso_Deve_Lancar()
    {
        var empresa = NovaEmpresa(NichoTipo.Distribuidora);
        Assert.Throws<InvalidOperationException>(() => empresa.ValidarOperacaoPorNicho(true, true));
    }

    [Fact]
    public void ValidarOperacaoPorNicho_Feira_Sem_VendaPorPeso_Deve_Lancar()
    {
        var empresa = NovaEmpresa(NichoTipo.Feira);
        Assert.Throws<InvalidOperationException>(() => empresa.ValidarOperacaoPorNicho(true, false));
    }

    [Fact]
    public void ValidarOperacaoPorNicho_Cenarios_Validos()
    {
        var mercado = NovaEmpresa(NichoTipo.Mercado);
        mercado.ValidarOperacaoPorNicho(true, false);

        var dist = NovaEmpresa(NichoTipo.Distribuidora);
        dist.ValidarOperacaoPorNicho(true, false);

        var feira = NovaEmpresa(NichoTipo.Feira);
        feira.ValidarOperacaoPorNicho(true, true);
    }
}