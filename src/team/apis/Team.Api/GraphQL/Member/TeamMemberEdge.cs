using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberEdge")]
public class TeamMemberEdge(TeamMemberDetails node, string cursor) : Edge<TeamMemberDetails>(node, cursor);
