using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class BookingCheckoutSession : ReplicatedModelBaseWithDeleted
{
    public string CheckoutUrl { get; set; } = string.Empty;
    public PaymentStatus PaymentStatus { get; set; }
    public Booking Booking { get; set; } = new();
}
