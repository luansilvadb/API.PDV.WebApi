/// <summary>
/// Serviço de aplicação para operações de produtos.
/// </summary>
using API.PDV.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.PDV.Application;

/// <summary>
/// Orquestra operações de CRUD de produtos.
/// </summary>
public class ProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly LoggingService _logger;

    /// <summary>
    /// Construtor do serviço de produtos.
    /// </summary>
    public ProdutoService(IProdutoRepository produtoRepository, LoggingService logger)
    {
        _produtoRepository = produtoRepository;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo produto.
    /// </summary>
    public async Task<Produto?> CriarAsync(Produto produto)
    {
        try
        {
            await ResiliencePolicy.ExecuteAsync(() =>
                _produtoRepository.AddAsync(produto), _logger, 3);
            return produto;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Obtém um produto pelo ID.
    /// </summary>
    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        try
        {
            return await ResiliencePolicy.ExecuteAsync(() =>
                _produtoRepository.GetByIdAsync(id), _logger, 3);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Lista todos os produtos.
    /// </summary>
    public async Task<IEnumerable<Produto>> ListarAsync()
    {
        try
        {
            return await ResiliencePolicy.ExecuteAsync(() =>
                _produtoRepository.GetAllAsync(), _logger, 3);
        }
        catch
        {
            return new List<Produto>();
        }
    }

    /// <summary>
    /// Atualiza um produto existente.
    /// </summary>
    public async Task AtualizarAsync(Produto produto)
    {
        await ResiliencePolicy.ExecuteAsync(() =>
            _produtoRepository.UpdateAsync(produto), _logger, 3);
    }

    /// <summary>
    /// Remove um produto pelo ID.
    /// </summary>
    public async Task RemoverAsync(Guid id)
    {
        await ResiliencePolicy.ExecuteAsync(() =>
            _produtoRepository.DeleteAsync(id), _logger, 3);
    }
}
