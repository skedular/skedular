using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Identity : ReplicatedModelBase, IIdentityDetails
{
    public Customer Customer { get; set; } = new();
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
}
