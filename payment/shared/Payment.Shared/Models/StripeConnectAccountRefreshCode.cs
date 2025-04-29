using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class StripeConnectAccountRefreshCode : ModelBaseWithDeleted
{
    public string Code { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;

    public StripeConnectAccount StripeConnectAccount { get; set; } = new();
}
