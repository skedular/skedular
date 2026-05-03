using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("CustomerDetails")]
[EntityKey("id")]
public class CustomerDetails(string id) : Node(id);
