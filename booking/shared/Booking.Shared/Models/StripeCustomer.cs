using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class StripeCustomer : ModelBaseWithDeleted
{
    public string StripeCustomerId { get; set; } = string.Empty;
    public string StripeAccountId { get; set; } = string.Empty;
    public Organization? Organization { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<StripeCheckoutSession> StripeCheckoutSessions { get; set; } = [];
}
