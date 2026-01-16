using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public CustomerType? Type { get; set; }

    public ICollection<Identity> Identities { get; set; } = [];
}
