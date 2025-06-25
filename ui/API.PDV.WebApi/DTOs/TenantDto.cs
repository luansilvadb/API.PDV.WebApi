using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace API.PDV.WebApi.DTOs
{
    /// <summary>
    /// DTO para cadastro e atualização de locatários (tenants).
    /// </summary>
    public class TenantDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "O nome deve ter até 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "O slug deve ter até 50 caracteres.")]
        [RegularExpression(@"^[a-z0-9\-]+$", ErrorMessage = "Slug deve conter apenas letras minúsculas, números e hífens.")]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "Documento deve ter entre 11 e 14 dígitos.")]
        [RegularExpression(@"^(\d{11}|\d{14})$", ErrorMessage = "Documento deve ser um CPF (11 dígitos) ou CNPJ (14 dígitos) numérico.")]
        public string Documento { get; set; } = string.Empty;
    }
}