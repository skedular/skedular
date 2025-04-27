using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class ProductVersion : ModelBase
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public PriceUnit PriceUnit { get; set; }
    public Currency Currency { get; set; }
    public Product Product { get; set; } = new();
    public OrganizationStripeConnectAccount? OrganizationStripeConnectAccount { get; set; }
    public virtual StripeProduct? StripeProduct { get; set; }
    public virtual StripePrice? StripePrice { get; set; }
}
