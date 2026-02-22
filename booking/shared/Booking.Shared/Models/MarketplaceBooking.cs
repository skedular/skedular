using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class MarketplaceBooking : ModelBase
{
    public PaymentStatus PaymentStatus { get; set; }
    public bool IsPaymentRequired { get; set; }
    public ICollection<ProductVersionLineItem> LineItems { get; set; } = [];
    public PaymentMethod PaymentMethod { get; set; }
    public DateTimeOffset PaymentExpiry { get; set; }
    public decimal? TotalAmountExcludeTax { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? TaxRatePercentage { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? InvoiceUrl { get; set; }
    public string? InvoiceNumber { get; set; }
    public ICollection<string> InvoiceEmailList { get; set; } = [];
    public Booking? Booking { get; set; }
    public RecurringBooking? RecurringBooking { get; set; }
    public ICollection<ProductVersion> ProductVersions { get; set; } = [];
    public Customer? PaidByCustomer { get; set; }
    public Organization? PaidByOrganization { get; set; }
    public StripeCheckoutSession? StripeCheckoutSession { get; set; }
}
