using Enterprise.Shared.Pagination;
using HotChocolate;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("OrganizationTagOrderInput")]
public class OrganizationTagOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public OrganizationTagOrderField Field { get; set; }
}
