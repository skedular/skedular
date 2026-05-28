using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class OrganizationSsoSetting : ReplicatedModelBase
{
    public bool IsActive { get; set; }
    public string EntityId { get; set; } = string.Empty;
    public string LoginUrl { get; set; } = string.Empty;
    public string AppFederationMetadataUrl { get; set; } = string.Empty;
    public Organization Organization { get; set; } = new();
}
