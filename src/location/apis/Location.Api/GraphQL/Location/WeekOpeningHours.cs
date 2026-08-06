using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("WeekOpeningHours")]
public class WeekOpeningHours
{
    [GraphQLName("monday")]
    public OpeningHoursDetails Monday { get; set; } = new();

    [GraphQLName("tuesday")]
    public OpeningHoursDetails Tuesday { get; set; } = new();

    [GraphQLName("wednesday")]
    public OpeningHoursDetails Wednesday { get; set; } = new();

    [GraphQLName("thursday")]
    public OpeningHoursDetails Thursday { get; set; } = new();

    [GraphQLName("friday")]
    public OpeningHoursDetails Friday { get; set; } = new();

    [GraphQLName("saturday")]
    public OpeningHoursDetails Saturday { get; set; } = new();

    [GraphQLName("sunday")]
    public OpeningHoursDetails Sunday { get; set; } = new();
}
