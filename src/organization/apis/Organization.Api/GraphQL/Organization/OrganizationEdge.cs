using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationEdge")]
public class OrganizationEdge(OrganizationDetails node, string cursor) : Edge<OrganizationDetails>(node, cursor);
