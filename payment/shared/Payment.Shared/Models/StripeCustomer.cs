using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class StripeCustomer : ModelBaseWithDeleted
{
    public string StripeCustomerId { get; set; } = string.Empty;
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
}
