using System;
using System.Collections.Generic;

namespace API.PDV.Domain
{
    public class Venda
    {
        public Guid Id { get; set; }
        public Guid EmpresaId { get; set; }
        public DateTime DataHora { get; set; }
        public decimal Total { get; set; }
        public decimal Troco { get; set; }
        public List<ItemVenda> Itens { get; set; } = new();
        public List<Pagamento> Pagamentos { get; set; } = new();
        public bool Finalizada { get; set; }
        // Regras de negócio específicas por nicho podem ser aplicadas via métodos
    }
}
