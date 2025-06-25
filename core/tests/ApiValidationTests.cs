using System.Net;
using System.Net.Http.Json;
using API.PDV.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.PDV.Domain.Tests
{
    public class ApiValidationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiValidationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("", 0.0, null)] // Nome vazio, preço zero
        [InlineData("Produto Inválido", -10.0, null)] // Preço negativo
        [InlineData("Produto Muito Longo Nome Produto Muito Longo Nome Produto Muito Longo Nome Produto Muito Longo Nome Produto Muito Longo Nome", 10.0, null)] // Nome > 100 chars
        public async Task ProdutoDto_DeveRetornarBadRequestParaDadosInvalidos(string nome, decimal preco, string? categoria)
        {
            var dto = new ProdutoDto
            {
                Nome = nome,
                Preco = preco,
                Categoria = categoria
            };

            var response = await _client.PostAsJsonAsync("/api/produto", dto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData(0, "")]
        [InlineData(-1, "Lote!@#")]
        public async Task EstoqueDto_DeveRetornarBadRequestParaDadosInvalidos(decimal quantidade, string lote)
        {
            var dto = new EstoqueDto
            {
                ProdutoId = Guid.NewGuid(),
                Quantidade = quantidade,
                Lote = lote
            };

            var response = await _client.PostAsJsonAsync("/api/estoque", dto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("", "slug-invalido!", "123")]
        [InlineData("Empresa", "slug", "abc")]
        public async Task TenantDto_DeveRetornarBadRequestParaDadosInvalidos(string nome, string slug, string documento)
        {
            var dto = new TenantDto
            {
                Nome = nome,
                Slug = slug,
                Documento = documento
            };

            var response = await _client.PostAsJsonAsync("/api/tenants", dto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}