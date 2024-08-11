using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class TenantMember : ModelBaseWithDeleted
{
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? JobTitle { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? PrincipalName { get; set; }
}
