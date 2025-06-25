using API.PDV.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.PDV.Infra
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AppDbContext _context;

        public TenantRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant?> GetBySlugAsync(string slug)
        {
            return await _context.Set<Tenant>().FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _context.Set<Tenant>().ToListAsync();
        }

        public async Task AddAsync(Tenant tenant)
        {
            await _context.Set<Tenant>().AddAsync(tenant);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsSchemaAsync(string schemaName)
        {
            var sql = $"SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = @schemaName";
            int count = 0;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = @schemaName";
                var param = command.CreateParameter();
                param.ParameterName = "@schemaName";
                param.Value = schemaName;
                command.Parameters.Add(param);

                await _context.Database.OpenConnectionAsync();
                var result = await command.ExecuteScalarAsync();
                count = result != null ? Convert.ToInt32(result) : 0;
                await _context.Database.CloseConnectionAsync();
            }
            return count > 0;
        }

        public async Task CreateSchemaAsync(string schemaName)
        {
            var sql = $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\"";
            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
