using System;
using System.Threading.Tasks;
using Xunit;
using API.PDV.Infra;
using API.PDV.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace API.PDV.Tests
{
    /// <summary>
    /// Testes de integração para Venda, usando schema temporário multitenant.
    /// </summary>
    public class VendaIntegrationTests : IClassFixture<TestSchemaFixture>
    {
        private readonly TestSchemaFixture _fixture;

        public VendaIntegrationTests(TestSchemaFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "Fluxo completo de venda com sucesso")]
        public async Task DeveRealizarVendaComSucesso()
        {
            var vendaRepo = _fixture.ServiceProvider.GetRequiredService<VendaRepository>();
            var itemRepo = _fixture.ServiceProvider.GetRequiredService<ItemVendaRepository>();
            var pagamentoRepo = _fixture.ServiceProvider.GetRequiredService<PagamentoRepository>();

            var venda = new Venda
            {
                Id = Guid.NewGuid(),
                EmpresaId = Guid.NewGuid(),
                Data = DateTime.UtcNow,
                Total = 0
            };

            await vendaRepo.AddAsync(venda);

            var item = new ItemVenda
            {
                Id = Guid.NewGuid(),
                VendaId = venda.Id,
                ProdutoId = Guid.NewGuid(),
                Quantidade = 2,
                PrecoUnitario = 10
            };

            await itemRepo.AddAsync(item);

            venda.Total = item.Quantidade * item.PrecoUnitario;
            await vendaRepo.UpdateAsync(venda);

            var pagamento = new Pagamento
            {
                Id = Guid.NewGuid(),
                VendaId = venda.Id,
                Valor = venda.Total,
                FormaPagamento = "Dinheiro"
            };

            await pagamentoRepo.AddAsync(pagamento);

            var vendaFinal = await vendaRepo.GetByIdAsync(venda.Id);
            Assert.NotNull(vendaFinal);
            Assert.Equal(venda.Total, vendaFinal.Total);
        }

        [Fact(DisplayName = "Erro ao adicionar item em venda inexistente")]
        public async Task DeveFalharItemVendaInexistente()
        {
            var itemRepo = _fixture.ServiceProvider.GetRequiredService<ItemVendaRepository>();
            var item = new ItemVenda
            {
                Id = Guid.NewGuid(),
                VendaId = Guid.NewGuid(), // Venda não existe
                ProdutoId = Guid.NewGuid(),
                Quantidade = 1,
                PrecoUnitario = 5
            };

            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await itemRepo.AddAsync(item);
            });
        }

        [Fact(DisplayName = "Rollback em falha de pagamento")]
        public async Task DeveFazerRollbackPagamento()
        {
            var context = _fixture.ServiceProvider.GetRequiredService<AppDbContext>();
            var vendaRepo = _fixture.ServiceProvider.GetRequiredService<VendaRepository>();
            var pagamentoRepo = _fixture.ServiceProvider.GetRequiredService<PagamentoRepository>();

            var venda = new Venda
            {
                Id = Guid.NewGuid(),
                EmpresaId = Guid.NewGuid(),
                Data = DateTime.UtcNow,
                Total = 50
            };

            await vendaRepo.AddAsync(venda);

            using var tx = await context.Database.BeginTransactionAsync();
            var pagamento = new Pagamento
            {
                Id = Guid.NewGuid(),
                VendaId = venda.Id,
                Valor = 50,
                FormaPagamento = "Cartão"
            };

            await pagamentoRepo.AddAsync(pagamento);

            // Força erro: pagamento duplicado (mesmo ID)
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await pagamentoRepo.AddAsync(pagamento);
            });
            await tx.RollbackAsync();

            var encontrado = await pagamentoRepo.ObterPorIdAsync(pagamento.Id);
            Assert.Null(encontrado);
        }

        [Fact(DisplayName = "Concorrência: dois pagamentos para mesma venda")]
        public async Task DeveFalharConcorrenciaPagamento()
        {
            var pagamentoRepo = _fixture.ServiceProvider.GetRequiredService<PagamentoRepository>();
            var vendaId = Guid.NewGuid();

            var pagamento1 = new Pagamento
            {
                Id = Guid.NewGuid(),
                VendaId = vendaId,
                Valor = 30,
                FormaPagamento = "Pix"
            };
            var pagamento2 = new Pagamento
            {
                Id = Guid.NewGuid(),
                VendaId = vendaId,
                Valor = 30,
                FormaPagamento = "Pix"
            };

            await pagamentoRepo.AddAsync(pagamento1);
            // Supondo restrição de um pagamento por venda
            await Assert.ThrowsAsync<DbUpdateException>(async () =>
            {
                await pagamentoRepo.AddAsync(pagamento2);
            });
        }
    }
}