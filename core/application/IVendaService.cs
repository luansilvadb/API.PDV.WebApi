using System;
using System.Threading.Tasks;
using API.PDV.Domain;

namespace API.PDV.Application
{
    public interface IVendaService
    {
        Task<Venda> IniciarVendaAsync(Guid empresaId);
        Task<ItemVenda> AdicionarItemAsync(Guid vendaId, Guid produtoId, decimal quantidade);
        Task<Venda> FecharVendaAsync(Guid vendaId);
        Task<Pagamento> ReceberPagamentoAsync(Guid vendaId, decimal valor, string formaPagamento);
    }
}
