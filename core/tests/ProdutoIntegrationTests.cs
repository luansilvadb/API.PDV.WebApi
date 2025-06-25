using System;
using System.Threading.Tasks;
using Xunit;
using API.PDV.Infra;
using API.PDV.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace API.PDV.Tests
{
    /// <summary>
    /// Testes de integração para Produto, usando schema temporário multitenant.
    /// </summary>
    public class ProdutoIntegrationTests : IClassFixture<TestSchemaFixture>
    {
        private readonly TestSchemaFixture _fixture;

        public ProdutoIntegrationTests(TestSchemaFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Criar produto com sucesso")]
        public async Task DeveCriarProdutoComSucesso()
        {
            var repo = _fixture.ServiceProvider.GetRequiredService<ProdutoRepository>();
            var produto = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Produto Teste",
                Preco = 10.5m,
                EstoqueMinimo = 2
            };

            await repo.AddAsync(produto);
            var encontrado = await repo.GetByIdAsync(produto.Id);

            Assert.NotNull(encontrado);
            Assert.Equal("Produto Teste", encontrado.Nome);
        }

        [Fact(DisplayName = "Erro ao criar produto duplicado")]
        public async Task DeveFalharProdutoDuplicado()
        {
            var repo = _fixture.ServiceProvider.GetRequiredService<ProdutoRepository>();
            var produto = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Produto Único",
                Preco = 5,
                EstoqueMinimo = 1
            };

            await repo.AddAsync(produto);

            // Tenta adicionar novamente com mesmo ID
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await repo.AddAsync(produto);
            });
        }

        [Fact(DisplayName = "Rollback em falha de transação")]
        public async Task DeveFazerRollbackAoFalhar()
        {
            var context = _fixture.ServiceProvider.GetRequiredService<AppDbContext>();
            var repo = _fixture.ServiceProvider.GetRequiredService<ProdutoRepository>();
            var produto = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Rollback Teste",
                Preco = 20,
                EstoqueMinimo = 1
            };

            using var tx = await context.Database.BeginTransactionAsync();
            await repo.AddAsync(produto);
            // Força erro
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await repo.AddAsync(produto); // Duplicado
            });
            await tx.RollbackAsync();

            var encontrado = await repo.GetByIdAsync(produto.Id);
            Assert.Null(encontrado);
        }

        [Fact(DisplayName = "Concorrência: dois produtos com mesmo nome")]
        public async Task DeveFalharConcorrenciaNome()
        {
            var repo = _fixture.ServiceProvider.GetRequiredService<ProdutoRepository>();
            var produto1 = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Concorrente",
                Preco = 8,
                EstoqueMinimo = 1
            };
            var produto2 = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = "Concorrente",
                Preco = 9,
                EstoqueMinimo = 2
            };

            await repo.AddAsync(produto1);
            // Supondo restrição de nome único
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await repo.AddAsync(produto2);
            });
        }
    }
}