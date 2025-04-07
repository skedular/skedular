namespace Enterprise.Shared.Security.Sso.Models;

//Note: This is a minimal type for the response and will be enriched in the future if needed.
public class SamlResponse
{
    public string Destination { get; set; } = string.Empty; // AssertionConsumerServiceUrl
    public string InResponseTo { get; set; } = string.Empty; // OrganizationId
    public string? NameId { get; set; }
    public Dictionary<string, string> Roles { get; set; } = [];

    public string? SessionIndex { get; set; }
    public DateTimeOffset SessionNotOnOrAfter { get; set; }

    public string Issuer { get; set; } = string.Empty;
    public DateTimeOffset AuthnInstant { get; set; }
    public string? AuthnContext { get; set; }

    public string StatusCode { get; set; } = string.Empty;
    public string? NestedStatusCode { get; set; }
    public string? StatusMessage { get; set; }
}
