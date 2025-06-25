using System;

namespace API.PDV.Domain
{
    public class Pagamento
    {
        public Guid Id { get; set; }
        public Guid VendaId { get; set; }
        public decimal Valor { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        // Possível extensão: status, parcelamento, etc.
    }
}
