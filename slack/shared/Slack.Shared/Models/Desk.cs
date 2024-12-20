using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Desk : ReplicatedModelBase
{
    public string? Name { get; set; }
    public bool Deactivated { get; set; }
    public bool RequireBookingApproval { get; set; }
    public Location? Location { get; set; }
    public ICollection<OrganizationDeskType> OrganizationDeskTypes { get; set; } = [];
    public ICollection<OrganizationZone> OrganizationZones { get; set; } = [];
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
