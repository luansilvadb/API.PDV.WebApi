using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.PDV.Domain
{
    /// <summary>
    /// Interface para repositório de relatórios otimizados (Dapper/multitenant).
    /// </summary>
    public interface IRelatorioRepository
    {
        Task<IEnumerable<VendaResumoDto>> ObterVendasPorPeriodoAsync(DateTime inicio, DateTime fim);
        Task<IEnumerable<EstoqueAtualDto>> ObterEstoqueAtualAsync();
        Task<IEnumerable<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(DateTime inicio, DateTime fim, int top = 10);
    }

    public class VendaResumoDto
    {
        public Guid Id { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }

    public class EstoqueAtualDto
    {
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; }
        public decimal Quantidade { get; set; }
        public string Lote { get; set; }
    }

    public class ProdutoMaisVendidoDto
    {
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; }
        public decimal QuantidadeVendida { get; set; }
    }
}