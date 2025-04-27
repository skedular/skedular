using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class Booking : ReplicatedModelBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }

    public ICollection<Team> InvolvedTeams { get; set; } = [];
}
