using Enterprise.Shared.Pagination;
using HotChocolate;
using Team.Shared.Models;

namespace Team.Api.GraphQL.Team;

[GraphQLName("TeamOrderInput")]
public class TeamOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public TeamOrderField Field { get; set; }
}
