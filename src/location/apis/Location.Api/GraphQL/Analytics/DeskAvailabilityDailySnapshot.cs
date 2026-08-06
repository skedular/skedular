using HotChocolate;

namespace Location.Api.GraphQL.Analytics;

[GraphQLName("ResourceAvailabilityDailySnapshot")]
public class ResourceAvailabilityDailySnapshot
{
    [GraphQLName("date")]
    public DateTimeOffset Date { get; set; }

    [GraphQLName("resourceType")]
    public string ResourceType { get; set; } = string.Empty;

    [GraphQLName("availableCount")]
    public int AvailableCount { get; set; }

    [GraphQLName("unavailableCount")]
    public int UnavailableCount { get; set; }

    [GraphQLName("bookedCount")]
    public int BookedCount { get; set; }

    [GraphQLName("availableResourceNames")]
    public IEnumerable<string> AvailableResourceNames { get; set; } = [];

    [GraphQLName("unavailableResourceNames")]
    public IEnumerable<string> UnavailableResourceNames { get; set; } = [];

    [GraphQLName("bookedResourceNames")]
    public IEnumerable<string> BookedResourceNames { get; set; } = [];
}
