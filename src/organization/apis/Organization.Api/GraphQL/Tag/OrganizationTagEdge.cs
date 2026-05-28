using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("OrganizationTagEdge")]
public class OrganizationTagEdge(OrganizationTagDetails node, string cursor) : Edge<OrganizationTagDetails>(node, cursor);
