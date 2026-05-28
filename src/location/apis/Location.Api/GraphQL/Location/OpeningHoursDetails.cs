using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("OpeningHoursDetails")]
public class OpeningHoursDetails
{
    [GraphQLName("closed")] public bool Closed { get; set; }
    [GraphQLName("openAllDay")] public bool OpenAllDay { get; set; }
    [GraphQLName("from")] public string? From { get; set; }
    [GraphQLName("until")] public string? Until { get; set; }
}
