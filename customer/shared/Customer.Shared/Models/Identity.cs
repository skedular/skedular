using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Identity : ModelBase, IIdentityDetails
{
    public Customer Customer { get; set; }
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
}
