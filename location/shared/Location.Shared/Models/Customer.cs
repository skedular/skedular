using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public CustomerType? Type { get; set; }
    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
