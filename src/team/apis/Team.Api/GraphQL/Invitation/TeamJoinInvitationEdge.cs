using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Team.Api.GraphQL.Invitation;

[GraphQLName("TeamJoinInvitationEdge")]
public class TeamJoinInvitationEdge(InviteCustomerToJoinTeamDetails node, string cursor) : Edge<InviteCustomerToJoinTeamDetails>(node, cursor);
