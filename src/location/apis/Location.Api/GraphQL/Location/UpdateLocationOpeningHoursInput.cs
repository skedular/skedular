using HotChocolate;
using Location.Api.Models;

namespace Location.Api.GraphQL.Location;

[GraphQLName("UpdateLocationOpeningHoursInput")]
public class UpdateLocationOpeningHoursInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("fieldsToUpdate")]
    public HashSet<LocationOpeningHoursPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("weekOpeningHours")]
    public WeekOpeningHours WeekOpeningHours { get; set; } = new();
}
