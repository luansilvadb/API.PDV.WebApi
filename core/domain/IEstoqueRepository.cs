namespace API.PDV.Domain;

public interface IEstoqueRepository : IRepository<Estoque>
{
    Task<Estoque?> GetByProdutoIdAsync(Guid produtoId, string? lote = null);
    Task BaixarEstoqueAsync(Guid produtoId, decimal quantidade, string? lote = null);
}
