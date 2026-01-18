using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("LocationDetails")]
public class LocationDetails(string id) : Node(id);
