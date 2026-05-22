using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.Models;

[GraphQLName("PrivateBookingPatchField")]
public enum PrivateBookingPatchField
{
    Participants,
    Schedule,
    Notes,
    Category,
    Resources
}

[GraphQLName("MarketplaceBookingPatchField")]
public enum MarketplaceBookingPatchField
{
    Participants,
    Notes,
    Category
}

[GraphQLName("PrivateRecurringBookingPatchField")]
public enum PrivateRecurringBookingPatchField
{
    Participants,
    RequestedResources,
    Schedule,
    Recurrence,
    SkippedDates,
    Category
}

public record PrivateBookingPatchRequest(
    Shared.Models.Booking Booking,
    IReadOnlySet<PrivateBookingPatchField> FieldsToUpdate);

public record MarketplaceBookingPatchRequest(
    Shared.Models.Booking Booking,
    IReadOnlySet<MarketplaceBookingPatchField> FieldsToUpdate);

public record PrivateRecurringBookingPatchRequest(
    RecurringBooking RecurringBooking,
    IReadOnlySet<PrivateRecurringBookingPatchField> FieldsToUpdate);
