using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("LocationDetails")]
public class LocationDetails(string id) : Node(id);
