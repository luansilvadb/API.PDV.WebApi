namespace API.PDV.Domain;

public class Estoque
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public decimal Quantidade { get; set; } // Suporta unidade ou peso
    public string? Localizacao { get; set; } // Para feiras itinerantes
    public DateTime? DataValidade { get; set; } // Para controle de validade
    public string? Lote { get; set; }
    public string? Departamento { get; set; } // Para nicho Mercado
    public Guid TenantId { get; set; } // Isolamento por locatário
}
