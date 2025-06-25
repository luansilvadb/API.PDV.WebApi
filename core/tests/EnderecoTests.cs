using System;
using API.PDV.Domain;
using Xunit;

public class EnderecoTests
{
    [Fact]
    public void Deve_Criar_Endereco_Valido()
    {
        var endereco = new Endereco("Rua A", "123", "CidadeX", "SP", "01000-000");
        Assert.Equal("Rua A", endereco.Rua);
        Assert.Equal("123", endereco.Numero);
        Assert.Equal("CidadeX", endereco.Cidade);
        Assert.Equal("SP", endereco.Estado);
        Assert.Equal("01000-000", endereco.Cep);
        Assert.Contains("Rua A", endereco.ToString());
    }

    [Theory]
    [InlineData("", "1", "C", "E", "C")]
    [InlineData("R", "", "C", "E", "C")]
    [InlineData("R", "1", "", "E", "C")]
    [InlineData("R", "1", "C", "", "C")]
    [InlineData("R", "1", "C", "E", "")]
    public void Deve_Lancar_Excecao_Para_Campos_Obrigatorios(string rua, string numero, string cidade, string estado, string cep)
    {
        Assert.Throws<ArgumentException>(() => new Endereco(rua, numero, cidade, estado, cep));
    }
}