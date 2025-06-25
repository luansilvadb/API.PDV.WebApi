using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.PDV.Domain;

namespace API.PDV.Application
{
    public class VendaService
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly IItemVendaRepository _itemVendaRepository;
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly LoggingService _logger;

        public VendaService(
            IVendaRepository vendaRepository,
            IItemVendaRepository itemVendaRepository,
            IPagamentoRepository pagamentoRepository,
            IEstoqueRepository estoqueRepository,
            IProdutoRepository produtoRepository,
            LoggingService logger)
        {
            _vendaRepository = vendaRepository;
            _itemVendaRepository = itemVendaRepository;
            _pagamentoRepository = pagamentoRepository;
            _estoqueRepository = estoqueRepository;
            _produtoRepository = produtoRepository;
            _logger = logger;
        }

        public async Task<Venda> IniciarVendaAsync(Guid empresaId)
        {
            return await ResiliencePolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var venda = new Venda
                    {
                        Id = Guid.NewGuid(),
                        EmpresaId = empresaId,
                        DataHora = DateTime.UtcNow,
                        Finalizada = false
                    };
                    return await _vendaRepository.AdicionarAsync(venda);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao iniciar venda.");
                    // Fallback: retorna null ou lança exceção customizada
                    throw new ApplicationException("Erro ao iniciar venda. Tente novamente mais tarde.", ex);
                }
            }, _logger);
        }

        public async Task<ItemVenda> AdicionarItemAsync(Guid vendaId, Guid produtoId, decimal quantidade)
        {
            return await ResiliencePolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var produto = await _produtoRepository.GetByIdAsync(produtoId);
                    if (produto == null)
                        throw new Exception("Produto não encontrado");

                    var item = new ItemVenda
                    {
                        Id = Guid.NewGuid(),
                        ProdutoId = produtoId,
                        DescricaoProduto = produto.Descricao,
                        Quantidade = quantidade,
                        PrecoUnitario = produto.Preco
                    };
                    await _itemVendaRepository.AdicionarAsync(item);
                    // Baixa de estoque
                    await _estoqueRepository.BaixarEstoqueAsync(produtoId, quantidade);
                    return item;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao adicionar item à venda.");
                    throw new ApplicationException("Erro ao adicionar item à venda. Tente novamente.", ex);
                }
            }, _logger);
        }

        public async Task<Venda> FecharVendaAsync(Guid vendaId)
        {
            return await ResiliencePolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var venda = await _vendaRepository.ObterPorIdAsync(vendaId);
                    if (venda == null)
                        throw new Exception("Venda não encontrada");
                    venda.Finalizada = true;
                    // Cálculo de total
                    venda.Total = 0;
                    foreach (var item in await _itemVendaRepository.ListarPorVendaAsync(vendaId))
                        venda.Total += item.Total;
                    await _vendaRepository.AtualizarAsync(venda);
                    return venda;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao fechar venda.");
                    throw new ApplicationException("Erro ao fechar venda. Tente novamente.", ex);
                }
            }, _logger);
        }

        public async Task<Pagamento> ReceberPagamentoAsync(Guid vendaId, decimal valor, string formaPagamento)
        {
            return await ResiliencePolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var venda = await _vendaRepository.ObterPorIdAsync(vendaId);
                    if (venda == null)
                        throw new Exception("Venda não encontrada");

                    var pagamento = new Pagamento
                    {
                        Id = Guid.NewGuid(),
                        VendaId = vendaId,
                        Valor = valor,
                        FormaPagamento = formaPagamento,
                        DataHora = DateTime.UtcNow
                    };
                    await _pagamentoRepository.AdicionarAsync(pagamento);

                    // Atualiza troco
                    var pagamentos = await _pagamentoRepository.ListarPorVendaAsync(vendaId);
                    decimal totalPago = 0;
                    foreach (var p in pagamentos)
                        totalPago += p.Valor;
                    // Garante que o total da venda está atualizado
                    if (venda.Total == 0)
                    {
                        venda.Total = 0;
                        foreach (var item in await _itemVendaRepository.ListarPorVendaAsync(vendaId))
                            venda.Total += item.Total;
                    }
                    venda.Troco = totalPago > venda.Total ? totalPago - venda.Total : 0;
                    await _vendaRepository.AtualizarAsync(venda);
                    return pagamento;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Falha ao receber pagamento.");
                    throw new ApplicationException("Erro ao receber pagamento. Tente novamente.", ex);
                }
            }, _logger);
        }
    }
}
