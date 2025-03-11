using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Resource : ModelBaseWithDeleted
{
    public string Name { get; set; }
    public bool Inactive { get; set; }
    public bool RequireBookingApproval { get; set; }
    public string? Color { get; set; }
    public bool? IsOpeningHoursOverriden { get; set; }
    public OpeningHours? OpeningHours { get; set; }

    public Location Location { get; set; }
    public ICollection<OrganizationTag> Tags { get; set; } = [];
}
