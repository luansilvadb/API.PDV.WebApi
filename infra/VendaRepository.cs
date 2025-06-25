using API.PDV.Domain;
using Microsoft.EntityFrameworkCore;

namespace API.PDV.Infra;

public class VendaRepository : GenericRepository<Venda>, IVendaRepository
{
    private readonly AppDbContext _context;
    public VendaRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Venda?> ObterPorIdAsync(Guid id)
    {
        return await _context.Set<Venda>()
            .Include(v => v.Itens)
            .Include(v => v.Pagamentos)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Venda>> ListarAsync()
    {
        return await _context.Set<Venda>()
            .Include(v => v.Itens)
            .Include(v => v.Pagamentos)
            .ToListAsync();
    }

    public async Task<Venda> AdicionarAsync(Venda venda)
    {
        await _context.Set<Venda>().AddAsync(venda);
        await _context.SaveChangesAsync();
        return venda;
    }

    public async Task AtualizarAsync(Venda venda)
    {
        _context.Set<Venda>().Update(venda);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var venda = await ObterPorIdAsync(id);
        if (venda != null)
        {
            _context.Set<Venda>().Remove(venda);
            await _context.SaveChangesAsync();
        }
    }
}
