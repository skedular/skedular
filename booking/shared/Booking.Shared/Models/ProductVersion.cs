using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class ProductVersion : ModelBase
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public PriceUnit PriceUnit { get; set; }
    public decimal PricePerMinute { get; set; }
    public Currency Currency { get; set; }
    public int? MinDurationMinutes { get; set; }
    public int? MaxDurationMinutes { get; set; }
    public bool BookAllLocationResources { get; set; }
    public int RecurrenceWindowDays { get; set; }
    public bool RequireConsecutiveDays { get; set; }
    public int? MaxBookingSpreadDays { get; set; }
    public int NumberOfResourcesToBook { get; set; }
    public int MaxAllowedResourcesLockTimePaidViaCard { get; set; }
    public int MaxAllowedResourcesLockTimePaidViaBankTransfer { get; set; }
    public Product Product { get; set; } = new();
    public ICollection<OrganizationTag> ProductTags { get; set; } = [];
    public ICollection<OrganizationTag> LocationTags { get; set; } = [];
    public StripeProduct? StripeProduct { get; set; }
    public StripePrice? StripePrice { get; set; }
}
