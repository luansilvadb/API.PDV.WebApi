using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.PDV.Domain
{
    public interface IPagamentoRepository
    {
        Task<Pagamento?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Pagamento>> ListarPorVendaAsync(Guid vendaId);
        Task<Pagamento> AdicionarAsync(Pagamento pagamento);
        Task RemoverAsync(Guid id);
    }
}
