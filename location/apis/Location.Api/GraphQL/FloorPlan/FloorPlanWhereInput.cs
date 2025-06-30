using HotChocolate;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("FloorPlanWhereInput")]
public class FloorPlanWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
}
