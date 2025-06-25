// Exemplo de uso (integrar ao fluxo de criação de tenant):
// await tenantRepository.CreateSchemaAsync(schemaName);
// tenantMigrationService.MigrateSchema(schemaName);
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.PDV.Infra
{
    /// <summary>
    /// Serviço utilitário para executar migrations do Entity Framework Core em um schema específico (tenant).
    /// </summary>
    public interface ITenantMigrationService
    {
        /// <summary>
        /// Aplica as migrations do EF Core para o schema informado.
        /// </summary>
        /// <param name="schema">Nome do schema (ex: tenant_slug)</param>
        void MigrateSchema(string schema);
    }

    public class TenantMigrationService : ITenantMigrationService
    {
        private readonly IServiceProvider _serviceProvider;

        public TenantMigrationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Executa as migrations do EF Core para o schema informado.
        /// </summary>
        /// <param name="schema">Nome do schema (ex: tenant_slug)</param>
        public void MigrateSchema(string schema)
        {
            if (string.IsNullOrWhiteSpace(schema))
                throw new ArgumentException("Schema não pode ser nulo ou vazio.", nameof(schema));

            // Cria um escopo para garantir ciclo de vida correto dos serviços
            using var scope = _serviceProvider.CreateScope();
            var schemaAccessor = scope.ServiceProvider.GetRequiredService<ISchemaContextAccessor>();
            schemaAccessor.CurrentSchema = schema;

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
    }
}