using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Booking.Api.GraphQL.RecurringBooking;

[GraphQLName("RecurringBookingEdge")]
public class RecurringBookingEdge(RecurringBookingDetails node, string cursor) : Edge<RecurringBookingDetails>(node, cursor);
