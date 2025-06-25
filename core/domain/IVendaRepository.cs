using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.PDV.Domain
{
    public interface IVendaRepository
    {
        Task<Venda?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Venda>> ListarAsync();
        Task<Venda> AdicionarAsync(Venda venda);
        Task AtualizarAsync(Venda venda);
        Task RemoverAsync(Guid id);
    }
}
