using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Resource : ReplicatedModelBase
{
    public string? Name { get; set; }
    public bool Inactive { get; set; }
    public bool RequireBookingApproval { get; set; }
    public string? Color { get; set; }
    public int Capacity { get; set; }

    public ResourceType ResourceType { get; set; }
    public Location? Location { get; set; }
    public ICollection<OrganizationCustomTag> OrganizationCustomTags { get; set; } = [];
    public ICollection<OrganizationZone> OrganizationZones { get; set; } = [];
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
