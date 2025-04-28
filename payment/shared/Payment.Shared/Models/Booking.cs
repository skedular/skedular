using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class Booking : ReplicatedModelBaseWithDeleted
{
    public bool IsPaymentRequired { get; set; }
    public Customer? PaidByCustomer { get; set; }
    public Organization? PaidByOrganization { get; set; }
    public ICollection<BookingSchedule> Schedules { get; set; } = [];
    public ICollection<ProductVersionLineItem> LineItems { get; set; } = [];
    public StripeCheckoutSession? StripeCheckoutSessions { get; set; }
}
