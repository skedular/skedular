using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingConnection")]
public class BookingConnection : Connection<BookingEdge>;
