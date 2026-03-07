using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class ProductVersion : ModelBase
{
    public string Name { get; set; } = string.Empty;
    public Currency Currency { get; set; }
    public ICollection<ProductPricing> PricingOptions { get; set; } = [];
    public Product Product { get; set; } = new();
    public ICollection<OrganizationTag> ProductTags { get; set; } = [];
    public ICollection<StripeProduct> StripeProducts { get; set; } = [];
}
