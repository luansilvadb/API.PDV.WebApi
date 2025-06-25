using System;

namespace API.PDV.Domain
{
    public class Empresa
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public CpfCnpj Documento { get; set; }
        public Endereco Endereco { get; set; }
        public NichoTipo Nicho { get; set; }
        public Configuracao Configuracao { get; set; }
        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        // Exemplo: cálculo de preço final conforme nicho
        public decimal CalcularPrecoFinal(decimal precoBase, decimal peso = 0, int quantidade = 1)
        {
            switch (Nicho)
            {
                case NichoTipo.Mercado:
                    // Mercado pode vender por unidade ou peso
                    if (peso > 0)
                        return precoBase * peso;
                    return precoBase * quantidade;
                case NichoTipo.Distribuidora:
                    // Distribuidora vende por caixa/litro
                    return precoBase * quantidade;
                case NichoTipo.Feira:
                    // Feira vende apenas por peso
                    if (peso <= 0)
                        throw new InvalidOperationException("Feira só permite venda por peso.");
                    return precoBase * peso;
                default:
                    return precoBase * quantidade;
            }
        }

        // Exemplo: validação de operação conforme nicho
        public void ValidarOperacaoPorNicho(bool possuiValidade, bool vendaPorPeso)
        {
            switch (Nicho)
            {
                case NichoTipo.Mercado:
                    if (!possuiValidade)
                        throw new InvalidOperationException("Mercado exige controle de validade.");
                    break;
                case NichoTipo.Distribuidora:
                    if (vendaPorPeso)
                        throw new InvalidOperationException("Distribuidora não permite venda por peso.");
                    break;
                case NichoTipo.Feira:
                    if (!vendaPorPeso)
                        throw new InvalidOperationException("Feira só permite venda por peso.");
                    break;
            }
        }
    }
}
