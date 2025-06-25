using System;
using System.Threading.Tasks;
using Xunit;
using API.PDV.Infra;
using API.PDV.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace API.PDV.Tests
{
    /// <summary>
    /// Testes de integração para Estoque, usando schema temporário multitenant.
    /// </summary>
    public class EstoqueIntegrationTests : IClassFixture<TestSchemaFixture>
    {
        private readonly TestSchemaFixture _fixture;

        public EstoqueIntegrationTests(TestSchemaFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Entrada e saída de estoque com sucesso")]
        public async Task DeveRealizarEntradaESaidaComSucesso()
        {
            var repo = _fixture.ServiceProvider.GetRequiredService<EstoqueRepository>();
            var produtoId = Guid.NewGuid();
            var estoque = new Estoque
            {
                Id = Guid.NewGuid(),
                ProdutoId = produtoId,
                Quantidade = 10,
                Lote = "L1"
            };

            await repo.AddAsync(estoque);

            // Entrada
            estoque.Quantidade += 5;
            await repo.UpdateAsync(estoque);

            // Saída
            estoque.Quantidade -= 3;
            await repo.UpdateAsync(estoque);

            var encontrado = await repo.GetByIdAsync(estoque.Id);
            Assert.NotNull(encontrado);
            Assert.Equal(12, encontrado.Quantidade);
        }

        [Fact(DisplayName = "Erro ao retirar mais do que disponível")]
        public async Task DeveFalharSaidaEstoqueInsuficiente()
        {
            var repo = _fixture.ServiceProvider.GetRequiredService<EstoqueRepository>();
            var estoque = new Estoque
            {
                Id = Guid.NewGuid(),
                ProdutoId = Guid.NewGuid(),
                Quantidade = 2,
                Lote = "L2"
            };

            await repo.AddAsync(estoque);

            // Tenta retirar mais do que disponível
            estoque.Quantidade -= 5;
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await repo.UpdateAsync(estoque);
            });
        }

        [Fact(DisplayName = "Rollback em falha de transação de estoque")]
        public async Task DeveFazerRollbackEstoque()
        {
            var context = _fixture.ServiceProvider.GetRequiredService<AppDbContext>();
            var repo = _fixture.ServiceProvider.GetRequiredService<EstoqueRepository>();
            var estoque = new Estoque
            {
                Id = Guid.NewGuid(),
                ProdutoId = Guid.NewGuid(),
                Quantidade = 7,
                Lote = "L3"
            };

            using var tx = await context.Database.BeginTransactionAsync();
            await repo.AddAsync(estoque);

            // Força erro
            estoque.Quantidade -= 10;
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await repo.UpdateAsync(estoque);
            });
            await tx.RollbackAsync();

            var encontrado = await repo.GetByIdAsync(estoque.Id);
            Assert.Null(encontrado);
        }

        [Fact(DisplayName = "Concorrência: duas saídas simultâneas")]
        public async Task DeveFalharConcorrenciaSaida()
        {
            var repo = _fixture.ServiceProvider.GetRequiredService<EstoqueRepository>();
            var estoque = new Estoque
            {
                Id = Guid.NewGuid(),
                ProdutoId = Guid.NewGuid(),
                Quantidade = 5,
                Lote = "L4"
            };

            await repo.AddAsync(estoque);

            // Simula duas saídas concorrentes
            var saida1 = repo.GetByIdAsync(estoque.Id);
            var saida2 = repo.GetByIdAsync(estoque.Id);

            var e1 = await saida1;
            var e2 = await saida2;

            e1.Quantidade -= 3;
            e2.Quantidade -= 4;

            await repo.UpdateAsync(e1);
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await repo.UpdateAsync(e2);
            });
        }
    }
}