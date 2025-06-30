using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("TeamConnection")]
public class TeamConnection : Connection<TeamEdge>;
