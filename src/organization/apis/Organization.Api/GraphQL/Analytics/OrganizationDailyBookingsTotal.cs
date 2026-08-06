using HotChocolate;

namespace Organization.Api.GraphQL.Analytics;

[GraphQLName("OrganizationDailyBookingsTotal")]
public class OrganizationDailyBookingsTotal
{
    [GraphQLName("date")]
    public DateTimeOffset Date { get; set; }

    [GraphQLName("total")]
    public int Total { get; set; }
}
