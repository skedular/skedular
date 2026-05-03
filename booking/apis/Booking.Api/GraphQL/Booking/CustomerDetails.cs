using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("CustomerDetails")]
[Shareable]
public class CustomerDetails(string id) : Node(id);
