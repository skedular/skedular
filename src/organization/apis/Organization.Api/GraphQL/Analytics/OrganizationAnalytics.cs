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
