/// <summary>
/// Controller para registro e gerenciamento de tenants (locatários).
/// </summary>
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.PDV.Domain;
using API.PDV.Infra;
using API.PDV.WebApi.DTOs;

namespace API.PDV.WebApi
{
    /// <summary>
    /// Gerencia endpoints de registro de novos tenants e schemas.
    /// </summary>
    [ApiController]
    [Route("api/tenantmgmt")]
    public class TenantMgmtController : ControllerBase
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantMigrationService _tenantMigrationService;

        /// <summary>
        /// Construtor do controller de tenants.
        /// </summary>
        public TenantMgmtController(ITenantRepository tenantRepository, ITenantMigrationService tenantMigrationService)
        {
            _tenantRepository = tenantRepository;
            _tenantMigrationService = tenantMigrationService;
        }

        /// <summary>
        /// Registra um novo tenant e cria o schema correspondente.
        /// </summary>
        /// <param name="dto">Dados do tenant.</param>
        /// <returns>Mensagem de sucesso e dados do tenant criado.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterTenant([FromBody] TenantDto dto)
        {
            var schemaName = $"tenant_{dto.Slug}";

            if (await _tenantRepository.ExistsSchemaAsync(schemaName))
                return BadRequest("Schema já existe para este tenant.");

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = dto.Nome,
                Slug = dto.Slug,
                CreatedAt = DateTime.UtcNow
            };

            await _tenantRepository.CreateSchemaAsync(schemaName);
            await _tenantRepository.AddAsync(tenant);

            // Aqui pode-se rodar migrations específicas para o novo schema, se necessário

            return Ok(new { message = "Tenant registrado e schema criado.", tenant });
        }

        /// <summary>
        /// Executa a migração automática para o schema do tenant informado.
        /// </summary>
        /// <param name="schema">Nome do schema do tenant (ex: tenant_slug).</param>
        /// <returns>Mensagem de sucesso ou erro detalhado.</returns>
        [HttpPost("{schema}/migrate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 500)]
        public IActionResult MigrateTenant(string schema)
        {
            try
            {
                _tenantMigrationService.MigrateSchema(schema);
                return Ok(new { message = $"Migração aplicada para o tenant/schema '{schema}'." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
