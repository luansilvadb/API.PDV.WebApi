/// <summary>
/// Serviço de aplicação para operações de estoque.
/// </summary>
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.PDV.Domain;

namespace API.PDV.Application;

/// <summary>
/// Orquestra operações de estoque, incluindo CRUD e movimentações.
/// </summary>
public class EstoqueService
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly LoggingService _logger;

    /// <summary>
    /// Construtor do serviço de estoque.
    /// </summary>
    public EstoqueService(IEstoqueRepository estoqueRepository, LoggingService logger)
    {
        _estoqueRepository = estoqueRepository;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo registro de estoque.
    /// </summary>
    public async Task<Estoque> CriarAsync(Estoque estoque)
    {
        await ResiliencePolicy.ExecuteAsync(
            () => _estoqueRepository.AddAsync(estoque),
            _logger
        );
        return estoque;
    }

    /// <summary>
    /// Obtém um registro de estoque pelo ID.
    /// </summary>
    public async Task<Estoque?> ObterPorIdAsync(Guid id)
    {
        return await ResiliencePolicy.ExecuteAsync<Estoque?>(
            () => _estoqueRepository.GetByIdAsync(id),
            _logger
        );
    }

    /// <summary>
    /// Lista todos os registros de estoque.
    /// </summary>
    public async Task<IEnumerable<Estoque>> ListarAsync()
    {
        return await ResiliencePolicy.ExecuteAsync<IEnumerable<Estoque>>(
            () => _estoqueRepository.GetAllAsync(),
            _logger
        );
    }

    /// <summary>
    /// Atualiza um registro de estoque existente.
    /// </summary>
    public async Task AtualizarAsync(Estoque estoque)
    {
        await ResiliencePolicy.ExecuteAsync(
            () => _estoqueRepository.UpdateAsync(estoque),
            _logger
        );
    }

    /// <summary>
    /// Remove um registro de estoque pelo ID.
    /// </summary>
    public async Task RemoverAsync(Guid id)
    {
        await ResiliencePolicy.ExecuteAsync(
            () => _estoqueRepository.DeleteAsync(id),
            _logger
        );
    }

    /// <summary>
    /// Realiza entrada de estoque para um produto.
    /// </summary>
    public async Task EntradaAsync(Guid produtoId, decimal quantidade, string? lote = null)
    {
        await ResiliencePolicy.ExecuteAsync(async () =>
        {
            var estoque = await _estoqueRepository.GetByProdutoIdAsync(produtoId, lote);
            if (estoque == null)
            {
                estoque = new Estoque { ProdutoId = produtoId, Quantidade = quantidade, Lote = lote };
                await _estoqueRepository.AddAsync(estoque);
            }
            else
            {
                estoque.Quantidade += quantidade;
                await _estoqueRepository.UpdateAsync(estoque);
            }
        }, _logger);
    }

    /// <summary>
    /// Realiza saída de estoque para um produto.
    /// </summary>
    public async Task SaidaAsync(Guid produtoId, decimal quantidade, string? lote = null)
    {
        await ResiliencePolicy.ExecuteAsync(async () =>
        {
            var estoque = await _estoqueRepository.GetByProdutoIdAsync(produtoId, lote);
            if (estoque == null || estoque.Quantidade < quantidade)
                throw new InvalidOperationException("Estoque insuficiente.");
            estoque.Quantidade -= quantidade;
            await _estoqueRepository.UpdateAsync(estoque);
        }, _logger);
    }
}
