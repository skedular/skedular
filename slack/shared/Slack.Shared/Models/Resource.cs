using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Resource : ReplicatedModelBase
{
    public string? Name { get; set; }
    public bool Inactive { get; set; }
    public bool RequireBookingApproval { get; set; }
    public string? Color { get; set; }
    public int Capacity { get; set; }

    public ResourceType ResourceType { get; set; } = new();
    public Location? Location { get; set; }
    public IReadOnlyList<OrganizationCustomTag> CustomTags { get; set; } = [];
    public IReadOnlyList<OrganizationZone> Zones { get; set; } = [];
    public IReadOnlyList<OrganizationProductTag> ProductTags { get; set; } = [];
    public IReadOnlyList<Customer> PreferredByCustomers { get; set; } = [];
}
