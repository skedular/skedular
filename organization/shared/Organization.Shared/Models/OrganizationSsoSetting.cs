using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationSsoSetting : ModelBase
{
    public string EntityId { get; set; } = string.Empty;
    public string LoginUrl { get; set; } = string.Empty;
    public string AppFederationMetadataUrl { get; set; } = string.Empty;

    public Organization Organization { get; set; }
}
