using Api.Shared.Services.Models;
using Booking.Shared.GraphQL;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.GraphQL;
using HotChocolate.Subscriptions;

namespace Booking.Api.Services;

public class GraphQlTopicEventSender(
    ITopicEventSender topicEventSender,
    ICachedBookingService cachedBookingService,
    ICachedMarketplaceBookingSubscriptionService cachedMarketplaceBookingSubscriptionService,
    ISubscriptionKeyService subscriptionKeyService)
    : IGraphQlTopicEventSender
{
    public async Task RaiseGraphqlChangeAsync(string topicName, string id, CancellationToken cancellationToken)
    {
        switch (topicName)
        {
            case Constants.BookingTopicName:
                // Emit resource availability subscription keys before clearing the cache
                await RaiseResourceAvailabilityKeysAsync(id, cancellationToken);
                await cachedBookingService.RemoveByIdAsync(id, cancellationToken);

                break;

            case Constants.MarketplaceBookingSubscriptionTopicName:
                await cachedMarketplaceBookingSubscriptionService.RemoveByIdAsync(id, cancellationToken);

                break;
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);
    }

    private async Task RaiseResourceAvailabilityKeysAsync(string bookingId, CancellationToken cancellationToken)
    {
        var booking = await cachedBookingService.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            return;
        }

        var locationId = booking.InvolvedLocations.FirstOrDefault()?.Id;
        var organizationCustomDomain = booking.InvolvedOrganizations.FirstOrDefault()?.CustomDomain;
        if (organizationCustomDomain is null || locationId is null)
        {
            return;
        }

        var date = DateOnly.FromDateTime(booking.From.UtcDateTime);

        // Determine resource type from the first involved resource's tags
        var resourceType = booking.InvolvedResources
            .SelectMany(item => item.OrganizationTags)
            .FirstOrDefault(item => item.Type is not null && OrganizationTagTypeConstants.ResourceTagTypes.Contains(item.Type))
            ?.Type;

        var affectedKeys = subscriptionKeyService.AffectedKeys(
            organizationCustomDomain,
            locationId,
            null, // floorId — not tracked in v1
            null, // zoneId — omit to ensure broad key coverage
            resourceType,
            date);

        foreach (var key in affectedKeys)
        {
            await topicEventSender.SendAsync(key, bookingId, cancellationToken);
        }
    }
}
