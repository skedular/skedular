using Booking.Shared.Models;
using Enterprise.Shared.Pagination;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingOrderInput")]
public class BookingOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public BookingOrderField Field { get; set; }
}
