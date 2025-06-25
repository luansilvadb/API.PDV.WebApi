using Microsoft.EntityFrameworkCore;
using API.PDV.Domain;

namespace API.PDV.Infra
{
    public class AppDbContext : DbContext
    {
        private readonly ISchemaContextAccessor _schemaContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, ISchemaContextAccessor schemaContextAccessor)
            : base(options)
        {
            _schemaContextAccessor = schemaContextAccessor;
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var schema = _schemaContextAccessor.CurrentSchema ?? "public";
            modelBuilder.HasDefaultSchema(schema);

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("Tenants", schema);
                entity.HasKey(t => t.Id);
                entity.HasIndex(t => t.Slug).IsUnique();
                entity.Property(t => t.Name).IsRequired();
                entity.Property(t => t.Slug).IsRequired();
                entity.Property(t => t.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", schema);
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.TenantSlug).IsRequired();
                entity.Property(u => u.Role).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
