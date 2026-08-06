using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("BookingWindowDetails")]
public class BookingWindowDetails
{
    [GraphQLName("bookingId")]
    public string BookingId { get; set; } = string.Empty;

    [GraphQLName("from")]
    public DateTimeOffset From { get; set; }

    [GraphQLName("until")]
    public DateTimeOffset Until { get; set; }

    [GraphQLName("isRecurring")]
    public bool IsRecurring { get; set; }

    [GraphQLName("isCheckedIn")]
    public bool IsCheckedIn { get; set; }

    [GraphQLName("bookedByName")]
    public string? BookedByName { get; set; }

    [GraphQLName("bookedByUserId")]
    public string? BookedByUserId { get; set; }

    [GraphQLName("notes")]
    public string? Notes { get; set; }
}
