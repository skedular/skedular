using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Team.Api.GraphQL.Team;

[GraphQLName("LocationDetails")]
[Shareable]
public class LocationDetails(string id) : Node(id);
