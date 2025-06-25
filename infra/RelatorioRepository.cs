using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using API.PDV.Domain;

namespace API.PDV.Infra
{
    /// <summary>
    /// Serviço de relatórios otimizados usando Dapper e multitenancy por schema.
    /// </summary>
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly IDbConnection _connection;
        private readonly ISchemaContextAccessor _schemaContextAccessor;

        public RelatorioRepository(IDbConnection connection, ISchemaContextAccessor schemaContextAccessor)
        {
            _connection = connection;
            _schemaContextAccessor = schemaContextAccessor;
        }

        /// <summary>
        /// Relatório de vendas por período.
        /// </summary>
        public async Task<IEnumerable<VendaResumoDto>> ObterVendasPorPeriodoAsync(DateTime inicio, DateTime fim)
        {
            var schema = _schemaContextAccessor.CurrentSchema;
            var sql = $@"
                SELECT v.Id, v.DataVenda, v.Total, v.Status
                FROM {schema}.Venda v
                WHERE v.DataVenda BETWEEN @inicio AND @fim
                ORDER BY v.DataVenda DESC";
            return await _connection.QueryAsync<VendaResumoDto>(sql, new { inicio, fim });
        }

        /// <summary>
        /// Relatório de estoque atual por produto.
        /// </summary>
        public async Task<IEnumerable<EstoqueAtualDto>> ObterEstoqueAtualAsync()
        {
            var schema = _schemaContextAccessor.CurrentSchema;
            var sql = $@"
                SELECT e.ProdutoId, p.Nome, e.Quantidade, e.Lote
                FROM {schema}.Estoque e
                INNER JOIN {schema}.Produto p ON e.ProdutoId = p.Id";
            return await _connection.QueryAsync<EstoqueAtualDto>(sql);
        }

        /// <summary>
        /// Relatório dos produtos mais vendidos em um período.
        /// </summary>
        public async Task<IEnumerable<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(DateTime inicio, DateTime fim, int top = 10)
        {
            var schema = _schemaContextAccessor.CurrentSchema;
            var sql = $@"
                SELECT iv.ProdutoId, p.Nome, SUM(iv.Quantidade) AS QuantidadeVendida
                FROM {schema}.ItemVenda iv
                INNER JOIN {schema}.Produto p ON iv.ProdutoId = p.Id
                INNER JOIN {schema}.Venda v ON iv.VendaId = v.Id
                WHERE v.DataVenda BETWEEN @inicio AND @fim
                GROUP BY iv.ProdutoId, p.Nome
                ORDER BY QuantidadeVendida DESC
                LIMIT @top";
            return await _connection.QueryAsync<ProdutoMaisVendidoDto>(sql, new { inicio, fim, top });
        }
    }
}