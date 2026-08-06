using Enterprise.Shared.Pagination;
using HotChocolate;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationOrderInput")]
public class OrganizationOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public OrganizationOrderField Field { get; set; }
}
