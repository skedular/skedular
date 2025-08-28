using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Analytics;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationAnalytics?> OrganizationAnalyticsAsync(
        string? id,
        string? uniqueAlphanumericName,
        DateTimeOffset from,
        DateTimeOffset until,
        [Service] IOrganizationAnalyticsService organizationAnalyticsService,
        CancellationToken cancellationToken)
    {
        var organizationAnalytics = await organizationAnalyticsService.GetAnalyticsAsync(id, uniqueAlphanumericName, from, until, cancellationToken);
        return mapper.MapTo(organizationAnalytics.MemberAttendancePercentage, organizationAnalytics.DailyBookingsTotal);
    }
}
