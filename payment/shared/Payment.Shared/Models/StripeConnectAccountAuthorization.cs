using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class StripeConnectAccountAuthorization : ModelBase
{
    public bool IsAuthorized { get; set; }
    public StripeConnectAccount StripeConnectAccount { get; set; } = new();
}
