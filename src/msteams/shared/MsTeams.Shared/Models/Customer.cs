using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public CustomerType? Type { get; set; }

    public IReadOnlyList<Identity> Identities { get; set; } = [];
}
