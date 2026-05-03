using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public OrganizationTagType? Type { get; set; }
    public string? Color { get; set; }
    public Organization Organization { get; set; } = new();
    public IReadOnlyList<Customer> PreferredByCustomers { get; set; } = [];
    public IReadOnlyList<Location> Locations { get; set; } = [];
    public IReadOnlyList<ProductVersion> ProductVersionOrganizationTags { get; set; } = [];
}
