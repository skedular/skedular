using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class MarketplaceBooking : ModelBase
{
    public PaymentStatus PaymentStatus { get; set; }
    public bool IsPaymentRequired { get; set; }
    public int Quantity { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ProductPricing ProductPricing { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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
    public ProductVersion ProductVersion { get; set; } = new();
    public Customer? PaidByCustomer { get; set; }
    public Organization? PaidByOrganization { get; set; }
    public StripeCheckoutSession? StripeCheckoutSession { get; set; }
}
