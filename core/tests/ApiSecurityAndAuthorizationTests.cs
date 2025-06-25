using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using API.PDV.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.PDV.Domain.Tests
{
    public class ApiSecurityAndAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiSecurityAndAuthorizationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private HttpClient GetClient(string? role = null)
        {
            var client = _factory.CreateClient();
            // Simulação simplificada: em ambiente real, gere JWT válido conforme configuração do projeto.
            if (role != null)
            {
                // Exemplo: Adicione um header Authorization simulado para testes.
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"fake-jwt-for-{role}");
            }
            return client;
        }

        [Fact]
        public async Task EndpointsProtegidos_Devolvem401_QuandoNaoAutenticado()
        {
            var client = GetClient();

            var produtoResponse = await client.PostAsJsonAsync("/api/produto", new ProdutoDto { Nome = "Teste", Preco = 1 });
            Assert.Equal(HttpStatusCode.Unauthorized, produtoResponse.StatusCode);

            var estoqueResponse = await client.PostAsJsonAsync("/api/estoque", new EstoqueDto { ProdutoId = Guid.NewGuid(), Quantidade = 1 });
            Assert.Equal(HttpStatusCode.Unauthorized, estoqueResponse.StatusCode);
        }

        [Fact]
        public async Task EndpointsDeEscrita_Devolvem403_QuandoAutenticadoSemRole()
        {
            var client = GetClient("Usuario"); // Role não autorizada

            var produtoResponse = await client.PostAsJsonAsync("/api/produto", new ProdutoDto { Nome = "Teste", Preco = 1 });
            Assert.Equal(HttpStatusCode.Forbidden, produtoResponse.StatusCode);

            var estoqueResponse = await client.PostAsJsonAsync("/api/estoque", new EstoqueDto { ProdutoId = Guid.NewGuid(), Quantidade = 1 });
            Assert.Equal(HttpStatusCode.Forbidden, estoqueResponse.StatusCode);
        }

        [Fact]
        public async Task EndpointsDeEscrita_Autorizam_AdminOuGerente()
        {
            var client = GetClient("Admin");

            var produtoResponse = await client.PostAsJsonAsync("/api/produto", new ProdutoDto { Nome = "Teste", Preco = 1 });
            Assert.NotEqual(HttpStatusCode.Unauthorized, produtoResponse.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, produtoResponse.StatusCode);

            var estoqueResponse = await client.PostAsJsonAsync("/api/estoque", new EstoqueDto { ProdutoId = Guid.NewGuid(), Quantidade = 1 });
            Assert.NotEqual(HttpStatusCode.Unauthorized, estoqueResponse.StatusCode);
            Assert.NotEqual(HttpStatusCode.Forbidden, estoqueResponse.StatusCode);
        }

        [Fact]
        public async Task EndpointsPublicos_PermitidosSemAutenticacao()
        {
            var client = GetClient();

            var tenantResponse = await client.PostAsJsonAsync("/api/tenants", new TenantDto { Nome = "Empresa", Slug = "empresa", Documento = "123456789" });
            Assert.NotEqual(HttpStatusCode.Unauthorized, tenantResponse.StatusCode);

            var errorResponse = await client.GetAsync("/error");
            Assert.NotEqual(HttpStatusCode.Unauthorized, errorResponse.StatusCode);
        }
    }
}