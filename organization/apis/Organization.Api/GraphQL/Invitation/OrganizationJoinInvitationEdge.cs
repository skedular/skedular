using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("OrganizationJoinInvitationEdge")]
public class OrganizationJoinInvitationEdge(InviteCustomerToJoinOrganizationDetails node, string cursor)
    : Edge<InviteCustomerToJoinOrganizationDetails>(node, cursor);
