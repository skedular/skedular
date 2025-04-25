using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Identity : ReplicatedModelBase, IIdentityDetails
{
    public Customer Customer { get; set; }
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
}
