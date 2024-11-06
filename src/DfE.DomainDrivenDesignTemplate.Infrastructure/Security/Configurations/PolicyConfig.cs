namespace DfE.DomainDrivenDesignTemplate.Infrastructure.Security.Configurations
{
    public class PolicyConfig
    {
        public required string Name { get; set; }
        public required string Operator { get; set; } = "OR"; // "AND" or "OR"
        public required List<string> Roles { get; set; }
        public List<ClaimConfig>? Claims { get; set; }
    }
}
