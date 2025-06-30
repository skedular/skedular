using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("UpdateLocationOpeningHoursInput")]
public class UpdateLocationOpeningHoursInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("weekOpeningHours")] public WeekOpeningHours WeekOpeningHours { get; set; } = new();
}
