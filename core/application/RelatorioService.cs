using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.PDV.Domain;

namespace API.PDV.Application
{
    /// <summary>
    /// Serviço de aplicação para relatórios, orquestra o acesso ao repositório de relatórios.
    /// </summary>
    public class RelatorioService
    {
        private readonly IRelatorioRepository _relatorioRepository;
        private readonly LoggingService _logger;

        public RelatorioService(IRelatorioRepository relatorioRepository, LoggingService logger)
        {
            _relatorioRepository = relatorioRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém o relatório de vendas por período.
        /// </summary>
        public async Task<IEnumerable<VendaResumoDto>> ObterVendasPorPeriodoAsync(DateTime inicio, DateTime fim)
        {
            return await ResiliencePolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _relatorioRepository.ObterVendasPorPeriodoAsync(inicio, fim);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao obter relatório de vendas por período.");
                    // Fallback: retorna lista vazia
                    return new List<VendaResumoDto>();
                }
            }, _logger);
        }

        /// <summary>
        /// Obtém o relatório de estoque atual.
        /// </summary>
        public async Task<IEnumerable<EstoqueAtualDto>> ObterEstoqueAtualAsync()
        {
            return await ResiliencePolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _relatorioRepository.ObterEstoqueAtualAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao obter relatório de estoque atual.");
                    // Fallback: retorna lista vazia
                    return new List<EstoqueAtualDto>();
                }
            }, _logger);
        }

        /// <summary>
        /// Obtém o relatório dos produtos mais vendidos em um período.
        /// </summary>
        public async Task<IEnumerable<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(DateTime inicio, DateTime fim, int top = 10)
        {
            return await ResiliencePolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await _relatorioRepository.ObterProdutosMaisVendidosAsync(inicio, fim, top);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao obter relatório de produtos mais vendidos.");
                    // Fallback: retorna lista vazia
                    return new List<ProdutoMaisVendidoDto>();
                }
            }, _logger);
        }
    }
}