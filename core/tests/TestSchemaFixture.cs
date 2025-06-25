using System;
using System.Threading.Tasks;
using API.PDV.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.PDV.Tests
{
    /// <summary>
    /// Fixture para criar/dropar schema temporário e rodar migrations para testes de integração multitenant.
    /// </summary>
    public class TestSchemaFixture : IAsyncLifetime
    {
        public string SchemaName { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }

        private IServiceScope _scope;

        public TestSchemaFixture()
        {
            SchemaName = $"tenant_test_{Guid.NewGuid():N}";
            var services = new ServiceCollection();

            // Configuração do contexto apontando para o schema temporário
            services.AddScoped<ISchemaContextAccessor>(_ => new SchemaContextAccessor { CurrentSchema = SchemaName });
            services.AddDbContext<AppDbContext>(options =>
            {
                // TODO: Ajustar a connection string conforme ambiente de testes
                options.UseNpgsql("Host=localhost;Port=5432;Database=pdv;Username=postgres;Password=postgres");
            });

            // Repositórios e serviços necessários
            services.AddScoped<TenantRepository>();
            services.AddScoped<TenantMigrationService>();

            ServiceProvider = services.BuildServiceProvider();
            _scope = ServiceProvider.CreateScope();
        }

        public async Task InitializeAsync()
        {
            var tenantRepo = _scope.ServiceProvider.GetRequiredService<TenantRepository>();
            var migrationService = _scope.ServiceProvider.GetRequiredService<TenantMigrationService>();

            // Cria schema temporário
            await tenantRepo.CreateSchemaAsync(SchemaName);

            // Executa migrations no schema
            migrationService.MigrateSchema(SchemaName);
        }

        public async Task DisposeAsync()
        {
            // Dropa schema ao final do teste
            using var context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var sql = $"DROP SCHEMA IF EXISTS \"{SchemaName}\" CASCADE";
            await context.Database.ExecuteSqlRawAsync(sql);
            _scope.Dispose();
        }
    }
}