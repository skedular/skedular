using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public string? Timezone { get; set; }

    public ICollection<Identity> Identities { get; set; } = [];
}
