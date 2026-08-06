using Enterprise.Shared.Pagination;
using HotChocolate;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationOrderInput")]
public class LocationOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public LocationOrderField Field { get; set; }
}
