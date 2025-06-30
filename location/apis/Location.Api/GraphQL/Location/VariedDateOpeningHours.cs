using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("VariedDateOpeningHours")]
public class VariedDateOpeningHours
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("openingHoursDetails")] public OpeningHoursDetails OpeningHoursDetails { get; set; } = new();
}
