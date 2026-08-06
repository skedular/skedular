using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("OpeningHours")]
public class OpeningHours
{
    [GraphQLName("weekOpeningHours")]
    public WeekOpeningHours WeekOpeningHours { get; set; } = new();

    [GraphQLName("closedDates")]
    public IEnumerable<DateTimeOffset> ClosedDates { get; set; } = [];

    [GraphQLName("datesWithVariedOpeningHours")]
    public IEnumerable<VariedDateOpeningHours> DatesWithVariedOpeningHours { get; set; } = [];
}
