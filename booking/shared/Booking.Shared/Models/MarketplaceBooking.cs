using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class MarketplaceBooking : ModelBase
{
    public PaymentStatus PaymentStatus { get; set; }
    public bool IsPaymentRequired { get; set; }
    public int Quantity { get; set; }
    public ProductPricing ProductPricing { get; set; } = ProductPricing.Empty(string.Empty);
    public PaymentMethod PaymentMethod { get; set; }
    public DateTimeOffset PaymentExpiry { get; set; }
    public decimal? TotalAmountExcludeTax { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? TaxRatePercentage { get; set; }
    public decimal? TotalAmount { get; set; }
    public Currency? Currency { get; set; }
    public string? InvoiceUrl { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? CheckoutReturnUrl { get; set; }
    public ICollection<string> InvoiceEmailList { get; set; } = [];
    public ProductPricingBillingMode BillingMode { get; set; }
    public Booking? Booking { get; set; }
    public RecurringBooking? RecurringBooking { get; set; }
    public MarketplaceBookingSubscription? MarketplaceBookingSubscription { get; set; }
    public ProductVersion ProductVersion { get; set; } = new();
    public Customer? PaidByCustomer { get; set; }
    public Organization? PaidByOrganization { get; set; }
    public StripeCheckoutSession? StripeCheckoutSession { get; set; }
}
