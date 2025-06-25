using System.Threading.Tasks;
using System.Collections.Generic;

namespace API.PDV.Domain
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetBySlugAsync(string slug);
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task AddAsync(Tenant tenant);
        // Métodos auxiliares para bootstrap, se necessário
        Task<bool> ExistsSchemaAsync(string schemaName);
        Task CreateSchemaAsync(string schemaName);
    }
}
