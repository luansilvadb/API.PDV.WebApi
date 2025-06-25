using API.PDV.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace API.PDV.Infra;

public class ProdutoRepository : GenericRepository<Produto>, IProdutoRepository
{
    private readonly AppDbContext _context;
    public ProdutoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    // Sobrescreve AddAsync para retornar o Produto criado
    public async Task<Produto> AddAsync(Produto entity)
    {
        var entry = await _context.Set<Produto>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    // Outros métodos específicos para Produto podem ser implementados aqui
}
