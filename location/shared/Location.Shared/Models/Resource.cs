using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Resource : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public bool Inactive { get; set; }
    public bool RequireBookingApproval { get; set; }
    public string? Color { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailableHoursOverridden { get; set; }
    public OpeningHours? AvailableHours { get; set; }

    public Location Location { get; set; } = new();
    public IReadOnlyList<OrganizationTag> Tags { get; set; } = [];
    public ResourcePosition? ResourcePosition { get; set; }
}
