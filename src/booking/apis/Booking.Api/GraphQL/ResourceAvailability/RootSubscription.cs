using System.Runtime.CompilerServices;
using Booking.Shared.Models;
using Booking.Shared.Services;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.ResourceAvailability;

[SubscriptionType]
public class RootSubscription
{
    public async IAsyncEnumerable<ResourceDayViewConnection> OnResourceAvailabilityChanged(
        string subscriptionKey,
        ResourceAvailabilityFilterInput filter,
        [Service]
        ITopicEventReceiver topicEventReceiver,
        [Service]
        IServiceProvider serviceProvider,
        [Service]
        ILogger<RootSubscription> logger,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        logger.LogDebug("ResourceAvailability subscription established. SubscriptionKey={SubscriptionKey}", subscriptionKey);

        var sourceStream = await topicEventReceiver.SubscribeAsync<string>(subscriptionKey, cancellationToken);

        // Yield initial snapshot
        yield return await GetCurrentViewAsync(filter, serviceProvider, cancellationToken);

        try
        {
            await foreach (var _ in sourceStream.ReadEventsAsync().WithCancellation(cancellationToken))
            {
                yield return await GetCurrentViewAsync(filter, serviceProvider, cancellationToken);
            }
        }
        finally
        {
            logger.LogDebug("ResourceAvailability subscription torn down. SubscriptionKey={SubscriptionKey}", subscriptionKey);
        }
    }

    [Subscribe(With = nameof(OnResourceAvailabilityChanged))]
    public ResourceDayViewConnection ResourceAvailability([EventMessage] ResourceDayViewConnection item) => item;

    private static async Task<ResourceDayViewConnection> GetCurrentViewAsync(
        ResourceAvailabilityFilterInput filter,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<IResourceAvailabilityDayViewService>();

        var domainFilter = new ResourceAvailabilityDayFilter
        {
            Date = filter.Date,
            OrganizationCustomDomain = filter.OrganizationCustomDomain,
            LocationIds = [.. filter.LocationIds],
            FloorId = filter.FloorId,
            ZoneId = filter.ZoneId,
            ResourceType = filter.ResourceType,
            Statuses = [.. filter.Statuses],
        };

        var result = await service.GetAsync(domainFilter, [], [], cancellationToken);

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
                    Notes = bookingWindow.Notes,
                }),
            }),
            SubscriptionKey = result.SubscriptionKey,
        };
    }
}
