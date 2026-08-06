using Booking.Shared.Models;
using Enterprise.Shared.Pagination;
using HotChocolate;

namespace Booking.Api.GraphQL.RecurringBooking;

[GraphQLName("RecurringBookingOrderInput")]
public class RecurringBookingOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public RecurringBookingOrderField Field { get; set; }
}
