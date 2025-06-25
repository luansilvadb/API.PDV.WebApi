using System;
using System.ComponentModel.DataAnnotations;

namespace API.PDV.WebApi.DTOs
{
    /// <summary>
    /// DTO para operações de estoque.
    /// </summary>
    public class EstoqueDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ProdutoId { get; set; }

        [Required]
        [Range(0, 100000.00, ErrorMessage = "Quantidade deve ser entre 0 e 100.000.")]
        public decimal Quantidade { get; set; }

        [StringLength(100, ErrorMessage = "O lote deve ter até 100 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9\-]*$", ErrorMessage = "Lote deve conter apenas letras, números e hífens.")]
        public string? Lote { get; set; }
    }
}