using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ProductVersionDetails")]
[EntityKey("id")]
public class ProductVersionDetails(string id) : Node(id);
