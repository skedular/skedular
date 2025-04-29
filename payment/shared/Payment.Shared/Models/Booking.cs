using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class Booking : ReplicatedModelBaseWithDeleted
{
    public bool IsPaymentRequired { get; set; }
    public ICollection<BookingSchedule> Schedules { get; set; } = [];
    public ICollection<ProductVersionLineItem> LineItems { get; set; } = [];
}
