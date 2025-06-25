using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.PDV.Domain
{
    public interface IItemVendaRepository
    {
        Task<ItemVenda?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<ItemVenda>> ListarPorVendaAsync(Guid vendaId);
        Task<ItemVenda> AdicionarAsync(ItemVenda itemVenda);
        Task RemoverAsync(Guid id);
    }
}
