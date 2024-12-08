namespace Enterprise.Shared.Security.Sso.Models;

public class SamlResponse
{
    // User Attributes
    public string Destination { get; set; } = string.Empty; // AssertionConsumerServiceUrl
    public string InResponseTo { get; set; } = string.Empty; // OrganizationId
    public string? NameId { get; set; }
    public string? Email { get; set; }
    public string ObjectId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];

    // Session Details
    public string? SessionIndex { get; set; }
    public DateTime SessionNotOnOrAfter { get; set; }

    // Metadata
    public string Issuer { get; set; } = string.Empty;
    public DateTime AuthnInstant { get; set; }
    public string? AuthnContext { get; set; }

    // Status
    public string StatusCode { get; set; } = string.Empty;
    public string? NestedStatusCode { get; set; }
    public string? StatusMessage { get; set; }
}
