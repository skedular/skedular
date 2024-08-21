using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class AzureTenantMember : ModelBaseWithDeleted
{
    public string? Email { get; set; }
    public string? Designation { get; set; }
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoUrl48 { get; set; }
    public string? PhotoUrl64 { get; set; }
    public string? PhotoUrl96 { get; set; }
    public string? PhotoUrl120 { get; set; }
    public string? PhotoUrl240 { get; set; }
    public string? PhotoUrl360 { get; set; }
    public string? PhotoUrl432 { get; set; }
    public string? PhotoUrl504 { get; set; }
    public string? PhotoUrl648 { get; set; }
}
