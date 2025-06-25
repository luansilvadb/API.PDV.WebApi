namespace API.PDV.Infra
{
    public interface ISchemaContextAccessor
    {
        string? CurrentSchema { get; set; }
    }

    public class SchemaContextAccessor : ISchemaContextAccessor
    {
        public string? CurrentSchema { get; set; }
    }
}
