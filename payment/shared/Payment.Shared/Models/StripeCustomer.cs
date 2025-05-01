using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class StripeCustomer : ModelBaseWithDeleted
{
    public string StripeCustomerId { get; set; } = string.Empty;
    public Organization Organization { get; set; } = new();
    public Customer Customer { get; set; } = new();
    public StripeConnectAccount? StripeConnectAccount { get; set; }
    public ICollection<StripeCheckoutSession> StripeCheckoutSessions { get; set; } = [];
}
