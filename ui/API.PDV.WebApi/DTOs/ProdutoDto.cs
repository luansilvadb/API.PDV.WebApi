using System;
using System.ComponentModel.DataAnnotations;

namespace API.PDV.WebApi.DTOs
{
    /// <summary>
    /// DTO para cadastro e atualização de produtos.
    /// </summary>
    public class ProdutoDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O nome deve ter até 100 caracteres.")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ0-9\s\-]+$", ErrorMessage = "Nome deve conter apenas letras, números, espaços e hífens.")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000.00, ErrorMessage = "Preço deve ser maior que zero e até 100.000.")]
        public decimal Preco { get; set; }

        [StringLength(50, ErrorMessage = "A categoria deve ter até 50 caracteres.")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ0-9\s\-]*$", ErrorMessage = "Categoria deve conter apenas letras, números, espaços e hífens.")]
        public string? Categoria { get; set; }

        [Range(0, 1000.00, ErrorMessage = "Peso deve ser entre 0 e 1000.")]
        public decimal Peso { get; set; }
    }
}