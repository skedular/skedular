using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("TeamDetails")]
public class TeamDetails(string id) : Node(id);
