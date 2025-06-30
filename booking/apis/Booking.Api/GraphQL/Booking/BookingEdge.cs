using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingEdge")]
public class BookingEdge(BookingDetails node, string cursor) : Edge<BookingDetails>(node, cursor);
