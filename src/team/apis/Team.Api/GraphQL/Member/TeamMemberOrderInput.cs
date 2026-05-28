using Enterprise.Shared.Pagination;
using HotChocolate;
using Team.Shared.Models;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberOrderInput")]
public class TeamMemberOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public TeamMemberOrderField Field { get; set; }
}
