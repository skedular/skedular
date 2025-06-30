using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberEdge")]
public class OrganizationMemberEdge(OrganizationMemberDetails node, string cursor) : Edge<OrganizationMemberDetails>(node, cursor);
