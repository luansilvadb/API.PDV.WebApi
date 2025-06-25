using System;

namespace API.PDV.Domain
{
    public class Configuracao
    {
        public Guid Id { get; set; }
        public bool PermiteVendaFiado { get; set; }
        public int DiasFechamentoCaixa { get; set; }
        public string? Observacoes { get; set; }
        public Guid EmpresaId { get; set; }
        public Empresa? Empresa { get; set; }
    }
}
