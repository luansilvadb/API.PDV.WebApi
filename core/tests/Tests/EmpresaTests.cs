using System;
using Xunit;

namespace API.PDV.Domain.Tests
{
    public class EmpresaTests
    {
        private Empresa CriarEmpresa(NichoTipo nicho)
        {
            return new Empresa
            {
                Id = Guid.NewGuid(),
                Nome = "Teste",
                Documento = new CpfCnpj("12345678901"),
                Endereco = new Endereco("Rua Teste", "123", "Cidade", "UF", "00000-000"),
                Nicho = nicho,
                Configuracao = new Configuracao(),
                TenantId = Guid.NewGuid()
            };
        }

        [Theory]
        [InlineData(NichoTipo.Mercado, 10, 2, 0, 20)]
        [InlineData(NichoTipo.Mercado, 10, 0, 3, 30)]
        [InlineData(NichoTipo.Distribuidora, 15, 0, 4, 60)]
        [InlineData(NichoTipo.Feira, 8, 2.5, 0, 20)]
        public void CalcularPrecoFinal_DeveRetornarValorCorreto(NichoTipo nicho, decimal precoBase, decimal peso, int quantidade, decimal esperado)
        {
            var empresa = CriarEmpresa(nicho);
            var resultado = empresa.CalcularPrecoFinal(precoBase, peso, quantidade);
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void CalcularPrecoFinal_FeiraSemPeso_DeveLancarExcecao()
        {
            var empresa = CriarEmpresa(NichoTipo.Feira);
            Assert.Throws<InvalidOperationException>(() => empresa.CalcularPrecoFinal(10, 0, 2));
        }

        [Fact]
        public void ValidarOperacaoPorNicho_MercadoSemValidade_DeveLancarExcecao()
        {
            var empresa = CriarEmpresa(NichoTipo.Mercado);
            Assert.Throws<InvalidOperationException>(() => empresa.ValidarOperacaoPorNicho(false, false));
        }

        [Fact]
        public void ValidarOperacaoPorNicho_DistribuidoraVendaPorPeso_DeveLancarExcecao()
        {
            var empresa = CriarEmpresa(NichoTipo.Distribuidora);
            Assert.Throws<InvalidOperationException>(() => empresa.ValidarOperacaoPorNicho(true, true));
        }

        [Fact]
        public void ValidarOperacaoPorNicho_FeiraVendaPorUnidade_DeveLancarExcecao()
        {
            var empresa = CriarEmpresa(NichoTipo.Feira);
            Assert.Throws<InvalidOperationException>(() => empresa.ValidarOperacaoPorNicho(true, false));
        }

        [Fact]
        public void ValidarOperacaoPorNicho_OperacoesValidas_NaoLancaExcecao()
        {
            var mercado = CriarEmpresa(NichoTipo.Mercado);
            mercado.ValidarOperacaoPorNicho(true, false);

            var dist = CriarEmpresa(NichoTipo.Distribuidora);
            dist.ValidarOperacaoPorNicho(true, false);

            var feira = CriarEmpresa(NichoTipo.Feira);
            feira.ValidarOperacaoPorNicho(true, true);
        }
    }
}
