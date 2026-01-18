using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ResourceDetails")]
public class ResourceDetails(string id) : Node(id);
