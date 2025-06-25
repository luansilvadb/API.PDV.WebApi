using API.PDV.Domain;
using Microsoft.EntityFrameworkCore;

namespace API.PDV.Infra;

public class EstoqueRepository : GenericRepository<Estoque>, IEstoqueRepository
{
    private readonly AppDbContext _context;
    public EstoqueRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Estoque?> GetByProdutoIdAsync(Guid produtoId, string? lote = null)
    {
        return await _context.Set<Estoque>()
            .FirstOrDefaultAsync(e => e.ProdutoId == produtoId && (lote == null || e.Lote == lote));
    }

    public async Task BaixarEstoqueAsync(Guid produtoId, decimal quantidade, string? lote = null)
    {
        var estoque = await GetByProdutoIdAsync(produtoId, lote);
        if (estoque == null)
            throw new Exception("Estoque não encontrado para o produto");
        if (estoque.Quantidade < quantidade)
            throw new Exception("Estoque insuficiente");
        estoque.Quantidade -= quantidade;
        await UpdateAsync(estoque);
    }
}
