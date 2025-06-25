namespace API.PDV.Domain
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty; // usado para nome do schema: tenant_{slug}
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
