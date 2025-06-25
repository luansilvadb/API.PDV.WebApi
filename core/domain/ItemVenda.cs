using System;

namespace API.PDV.Domain
{
    public class ItemVenda
    {
        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public string? DescricaoProduto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Total => Quantidade * PrecoUnitario;
        // Campos para regras de nicho: peso, validade, lote, etc.
    }
}
