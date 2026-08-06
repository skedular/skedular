using Enterprise.Shared.Pagination;
using HotChocolate;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberOrderInput")]
public class OrganizationMemberOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public OrganizationMemberOrderField Field { get; set; }
}
