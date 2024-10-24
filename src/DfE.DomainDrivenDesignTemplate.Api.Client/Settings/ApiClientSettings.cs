namespace DfE.DomainDrivenDesignTemplate.Api.Client.Settings
{
    public class ApiClientSettings
    {
        public string? BaseUrl { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Authority { get; set; }
        public string? Scope { get; set; }
        public ODataSettings? OData { get; set; }
    }

    public class ODataSettings
    {
        public string? BaseUrl { get; set; }
    }
}
