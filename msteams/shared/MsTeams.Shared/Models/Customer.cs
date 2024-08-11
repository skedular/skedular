using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public ICollection<Identity> Identities { get; set; } = [];
}
