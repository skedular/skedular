using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Enterprise.Shared;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.ResourceAvailability;

[QueryType]
public class RootQuery(IMapper mapper)
{
    [UseResolverScope]
    public async Task<IEnumerable<BookingResourceDetails>> AvailableResourcesAsync(
        AvailableResourcesWhereInput where,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        mapper.MapTo(
            await resourceService.GetAvailableResourcesAsync(
                where.OrganizationId,
                where.OrganizationCustomDomain,
                where.LocationId,
                where.From,
                where.Until,
                where.CustomTagIds.ToSafeCollection(),
                where.ZoneIds.ToSafeCollection(),
                where.ResourceIdsToInclude.ToSafeCollection(),
                where.ProductId,
                cancellationToken));

    [UseResolverScope]
    public async Task<int> AvailableResourcesCountAsync(
        AvailableResourcesCountWhereInput where,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        await resourceService.GetAvailableResourcesCountAsync(
            where.OrganizationId,
            where.OrganizationCustomDomain,
            where.LocationId,
            where.RequestedResourceIds.ToSafeCollection(),
            where.From,
            where.Until,
            where.CustomTagIds.ToSafeCollection(),
            where.ZoneIds.ToSafeCollection(),
            where.ProductId,
            cancellationToken);

    [UseResolverScope]
    public async Task<OrganizationAvailableResources> OrganizationResourcesAvailabilityAsync(
        OrganizationAvailableResourcesWhereInput where,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var (resourcesCount, availableResourcesCount) = await resourceService.GetOrganizationResourceAvailabilityAsync(
            where.OrganizationId,
            where.OrganizationCustomDomain,
            where.From,
            where.Until,
            cancellationToken);

        return new OrganizationAvailableResources { ResourcesCount = resourcesCount, AvailableResourcesCount = availableResourcesCount };
    }
}
