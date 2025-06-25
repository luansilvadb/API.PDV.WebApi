using System;
using API.PDV.Domain;
using Xunit;

public class CpfCnpjTests
{
    [Theory]
    [InlineData("12345678901")]
    [InlineData("123.456.789-01")]
    public void Deve_Criar_Cpf_Valido(string valor)
    {
        var cpf = new CpfCnpj(valor);
        Assert.Equal("12345678901", cpf.Valor);
    }

    [Theory]
    [InlineData("12345678000199")]
    [InlineData("12.345.678/0001-99")]
    public void Deve_Criar_Cnpj_Valido(string valor)
    {
        var cnpj = new CpfCnpj(valor);
        Assert.Equal("12345678000199", cnpj.Valor);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    public void Deve_Lancar_Excecao_Para_Valor_Invalido(string valor)
    {
        Assert.Throws<ArgumentException>(() => new CpfCnpj(valor));
    }

    [Fact]
    public void ToString_Deve_Retornar_Valor()
    {
        var cpf = new CpfCnpj("12345678901");
        Assert.Equal("12345678901", cpf.ToString());
    }
}