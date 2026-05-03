using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("TeamDetails")]
[EntityKey("id")]
public class TeamDetails(string id) : Node(id);
