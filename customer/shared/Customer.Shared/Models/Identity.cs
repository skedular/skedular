using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Identity : ModelBase, IIdentityDetails
{
    public Customer Customer { get; set; } = new();
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
}
