using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberConnection")]
public class TeamMemberConnection : Connection<TeamMemberEdge>;
