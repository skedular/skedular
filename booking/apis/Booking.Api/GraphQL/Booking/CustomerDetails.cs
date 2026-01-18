using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("CustomerDetails")]
public class CustomerDetails(string id) : Node(id);
