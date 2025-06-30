using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Team.Api.GraphQL.Team;

[GraphQLName("TeamEdge")]
public class TeamEdge(TeamDetails node, string cursor) : Edge<TeamDetails>(node, cursor);
