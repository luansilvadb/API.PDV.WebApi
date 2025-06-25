using System;
using API.PDV.Domain;
using Xunit;

public class PrecoTests
{
    [Fact]
    public void Deve_Criar_Preco_Valido()
    {
        var preco = new Preco(10.123m);
        Assert.Equal(10.12m, preco.Valor);
    }

    [Fact]
    public void Deve_Arredondar_Para_Duas_Casas()
    {
        var preco = new Preco(5.678m);
        Assert.Equal(5.68m, preco.Valor);
    }

    [Fact]
    public void Deve_Lancar_Excecao_Para_Valor_Negativo()
    {
        Assert.Throws<ArgumentException>(() => new Preco(-1));
    }

    [Fact]
    public void ToString_Deve_Retornar_Valor_Formatado()
    {
        var preco = new Preco(12.5m);
        Assert.Contains("12", preco.ToString());
    }
}