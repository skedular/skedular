using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.Analytics;

[GraphQLName("OrganizationAnalytics")]
public class OrganizationAnalytics
{
    [GraphQLName("memberAttendancePercentage")]
    public IEnumerable<OrganizationMemberAttendancePercentage> MemberAttendancePercentage { get; set; } = [];

    [GraphQLName("dailyBookingsTotals")] public IEnumerable<OrganizationDailyBookingsTotal> DailyBookingsTotals { get; set; } = [];
}

[GraphQLName("OrganizationMemberAttendancePercentage")]
public class OrganizationMemberAttendancePercentage
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("percentage")] public float Percentage { get; set; }
}

[GraphQLName("OrganizationDailyBookingsTotal")]
public class OrganizationDailyBookingsTotal
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("total")] public int Total { get; set; }
}
