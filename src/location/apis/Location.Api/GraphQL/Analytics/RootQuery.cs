using Enterprise.Shared;
using HotChocolate;
using HotChocolate.Types;
using Location.Api.GraphQL.Location;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Analytics;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<IEnumerable<LocationAnalytics>> LocationsAnalyticsAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        LocationWhereInput? where,
        IEnumerable<LocationOrderInput>? orderBy,
        [Service] ILocationAnalyticsService locationAnalyticsService,
        CancellationToken cancellationToken)
    {
        var locationsAnalytics = await locationAnalyticsService.GetAnalyticsAsync(
            from,
            until,
            new LocationSearchCriteria(
                null,
                where?.OrganizationCustomDomain,
                where is null ? [] : where.LocationIds.ToSafeCollection(),
                where?.NameContains,
                where is null ? [] : where.CustomTagIds.ToSafeCollection().Concat(where.ZoneIds.ToSafeCollection()).ToList(),
                null,
                where is null ? [] : where.Types.ToSafeCollection(),
                where?.SearchBoundaries,
                where?.NotContactedYet,
                where?.ResourceType,
                null,
                []),
            orderBy.ToSafeCollection().Select(item => new LocationOrder(item.Direction, item.Field)).ToList(),
            cancellationToken);

        return locationsAnalytics
            .Select(locationAnalytics =>
                graphQlMapper.MapTo(
                    locationAnalytics.Name,
                    locationAnalytics.DesksOccupancyPercentage,
                    locationAnalytics.DailyBookingsTotal,
                    locationAnalytics.RoomsOccupancyPercentage,
                    locationAnalytics.ResourceAvailabilitySnapshots));
    }
}
