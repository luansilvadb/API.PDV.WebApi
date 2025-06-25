namespace API.PDV.Domain;

public class Produto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? Departamento { get; set; } // Para nicho Mercado
    public decimal Preco { get; set; }
    public bool PorPeso { get; set; } // true: vendido por peso, false: por unidade
    public string? Embalagem { get; set; } // Ex: caixa, pacote, unidade
    public DateTime? Validade { get; set; } // Para controle de validade
    public string? Lote { get; set; } // Para controle de lote
    public string? Nicho { get; set; } // Mercado, Distribuidora, Feira
    public bool Ativo { get; set; } = true;
}
