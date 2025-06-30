using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Organization.Api.GraphQL;

[GraphQLName("OrganizationEdge")]
public class OrganizationEdge(OrganizationDetails node, string cursor) : Edge<OrganizationDetails>(node, cursor);
