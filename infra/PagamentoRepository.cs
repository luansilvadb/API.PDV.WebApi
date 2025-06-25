using API.PDV.Domain;
using Microsoft.EntityFrameworkCore;

namespace API.PDV.Infra;

public class PagamentoRepository : GenericRepository<Pagamento>, IPagamentoRepository
{
    private readonly AppDbContext _context;
    public PagamentoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Pagamento?> ObterPorIdAsync(Guid id)
    {
        return await _context.Set<Pagamento>().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Pagamento>> ListarPorVendaAsync(Guid vendaId)
    {
        return await _context.Set<Pagamento>().Where(p => p.VendaId == vendaId).ToListAsync();
    }

    public async Task<Pagamento> AdicionarAsync(Pagamento pagamento)
    {
        await _context.Set<Pagamento>().AddAsync(pagamento);
        await _context.SaveChangesAsync();
        return pagamento;
    }

    public async Task RemoverAsync(Guid id)
    {
        var pagamento = await ObterPorIdAsync(id);
        if (pagamento != null)
        {
            _context.Set<Pagamento>().Remove(pagamento);
            await _context.SaveChangesAsync();
        }
    }
}
