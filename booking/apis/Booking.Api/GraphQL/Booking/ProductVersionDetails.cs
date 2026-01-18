using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ProductVersionDetails")]
public class ProductVersionDetails(string id) : Node(id);
