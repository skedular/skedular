using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Booking : ReplicatedModelBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }

    public Organization Organization { get; set; }
    public ICollection<Organization> InvolvedOrganizations { get; set; } = [];
}
