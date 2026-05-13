using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Enterprise.Shared;
using HotChocolate;
using HotChocolate.Types;
using IResourceService = Booking.Api.Services.IResourceService;

namespace Booking.Api.GraphQL.ResourceAvailability;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public IEnumerable<ResourceAvailabilityClassificationDetails> ResourceAvailabilityStatuses() =>
    [
        new()
        {
            Type = ResourceAvailabilityClassification.Available,
            Name = ResourceAvailabilityClassification.Available.ToResourceAvailabilityClassificationName()
        },
        new()
        {
            Type = ResourceAvailabilityClassification.PartiallyBooked,
            Name = ResourceAvailabilityClassification.PartiallyBooked.ToResourceAvailabilityClassificationName()
        },
        new()
        {
            Type = ResourceAvailabilityClassification.FullyBooked,
            Name = ResourceAvailabilityClassification.FullyBooked.ToResourceAvailabilityClassificationName()
        },
        new()
        {
            Type = ResourceAvailabilityClassification.Unavailable,
            Name = ResourceAvailabilityClassification.Unavailable.ToResourceAvailabilityClassificationName()
        },
        new()
        {
            Type = ResourceAvailabilityClassification.Occupied,
            Name = ResourceAvailabilityClassification.Occupied.ToResourceAvailabilityClassificationName()
        },
        new()
        {
            Type = ResourceAvailabilityClassification.Blocked,
            Name = ResourceAvailabilityClassification.Blocked.ToResourceAvailabilityClassificationName()
        }
    ];

    [UseResolverScope]
    public async Task<ResourceDayViewConnection> ResourceDayViewsAsync(
        ResourceAvailabilityFilterInput filter,
        IEnumerable<ResourceAvailabilityOrderByInput> orderBy,
        [Service] IResourceAvailabilityDayViewService service,
        CancellationToken cancellationToken)
    {
        var domainFilter = new ResourceAvailabilityDayFilter
        {
            Date = filter.Date,
            OrganizationCustomDomain = filter.OrganizationCustomDomain,
            LocationIds = filter.LocationIds.ToList(),
            FloorId = filter.FloorId,
            ZoneId = filter.ZoneId,
            ResourceType = filter.ResourceType,
            Statuses = filter.Statuses.ToList()
        };

        var result = await service.GetAsync(
            domainFilter,
            orderBy.Select(item => new ResourceAvailabilityOrder(item.Direction, item.Field)).ToList(),
            [],
            cancellationToken);

        return new ResourceDayViewConnection
        {
            Items = result.Items.Select(item => new ResourceDayViewDetails
            {
                ResourceId = item.ResourceId,
                ResourceName = item.ResourceName,
                ResourceType = item.ResourceType,
                LocationId = item.LocationId,
                LocationName = item.LocationName,
                FloorId = item.FloorId,
                FloorName = item.FloorName,
                ZoneId = item.ZoneId,
                ZoneName = item.ZoneName,
                Date = item.Date,
                Status = item.Status,
                OpeningFrom = item.OpeningFrom,
                OpeningUntil = item.OpeningUntil,
                TotalOpeningMinutes = item.TotalOpeningMinutes,
                BookedMinutes = item.BookedMinutes,
                BookingWindows = item.BookingWindows.Select(bookingWindow => new BookingWindowDetails
                {
                    BookingId = bookingWindow.BookingId,
                    From = bookingWindow.From,
                    Until = bookingWindow.Until,
                    IsRecurring = bookingWindow.IsRecurring,
                    IsCheckedIn = bookingWindow.IsCheckedIn,
                    BookedByName = bookingWindow.BookedByName,
                    BookedByUserId = bookingWindow.BookedByUserId,
                    Notes = bookingWindow.Notes
                })
            }),
            SubscriptionKey = result.SubscriptionKey
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<BookingResourceDetails>> AvailableResourcesAsync(
        AvailableResourcesWhereInput where,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(
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
