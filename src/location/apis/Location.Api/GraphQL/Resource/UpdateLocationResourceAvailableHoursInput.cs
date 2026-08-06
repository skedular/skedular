using HotChocolate;
using Location.Api.GraphQL.Location;
using Location.Api.Models;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("UpdateLocationResourceAvailableHoursInput")]
public class UpdateLocationResourceAvailableHoursInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("fieldsToUpdate")]
    public HashSet<LocationResourceAvailableHoursPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("overrideAvailableHours")]
    public bool OverrideAvailableHours { get; set; }

    [GraphQLName("availableHours")]
    public WeekOpeningHours? AvailableHours { get; set; }
}
