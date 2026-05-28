using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class StripeCustomer : ModelBaseWithDeleted
{
    public string StripeCustomerId { get; set; } = string.Empty;
    public Customer Customer { get; set; } = new();
}
