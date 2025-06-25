using API.PDV.Domain;
using Microsoft.EntityFrameworkCore;

namespace API.PDV.Infra;

public class ItemVendaRepository : GenericRepository<ItemVenda>, IItemVendaRepository
{
    private readonly AppDbContext _context;
    public ItemVendaRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ItemVenda?> ObterPorIdAsync(Guid id)
    {
        return await _context.Set<ItemVenda>().FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<ItemVenda>> ListarPorVendaAsync(Guid vendaId)
    {
        return await _context.Set<ItemVenda>().Where(i => i.ProdutoId == vendaId).ToListAsync();
    }

    public async Task<ItemVenda> AdicionarAsync(ItemVenda itemVenda)
    {
        await _context.Set<ItemVenda>().AddAsync(itemVenda);
        await _context.SaveChangesAsync();
        return itemVenda;
    }

    public async Task RemoverAsync(Guid id)
    {
        var item = await ObterPorIdAsync(id);
        if (item != null)
        {
            _context.Set<ItemVenda>().Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}
