using Api.Shared.Clients.Events.Skedular.Booking.V1;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Enterprise.Shared.Sanitization;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Services.Cache;
using Location.Shared.Workflows;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Type;

namespace Location.Processors.Subscribers;

public class BookingSubscriber(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedLocationBookingAccessService cachedLocationBookingAccessService,
    ITemporalService temporalService) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.BookingUpserted:
                await ApplyBookingAccessDeltaAsync(@event, 1, cancellationToken);
                await SignalLocationBookingDerivedStateAsync(@event, cancellationToken);
                break;

            case Type.BookingDeleted:
                await ApplyBookingAccessDeltaAsync(@event, -1, cancellationToken);
                await SignalLocationBookingDerivedStateAsync(@event, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task ApplyBookingAccessDeltaAsync(Event @event, int delta, CancellationToken cancellationToken)
    {
        var eventRaisedAt = @event.Metadata.Time.ToDateTimeOffset();
        var changedFacts = new List<LocationBookingAccess>();
        foreach (var fact in ToAccessFacts(@event))
        {
            var existing = await repositoryFactory.LocationBookingAccessRepository.GetByCustomerLocationAndOrganizationAsync(
                fact.CustomerId,
                fact.LocationId,
                fact.OrganizationId,
                cancellationToken);

            if (existing?.EventRaisedAt is not null && existing.EventRaisedAt.Value >= eventRaisedAt)
            {
                continue;
            }

            if (existing is null)
            {
                if (delta <= 0)
                {
                    continue;
                }

                fact.Id = randomHelper.Generate();
                fact.ActiveBookingCount = delta;
                repositoryFactory.LocationBookingAccessRepository.Add(fact);
                changedFacts.Add(fact);
                continue;
            }

            existing.ActiveBookingCount = Math.Max(0, existing.ActiveBookingCount + delta);
            existing.EventRaisedAt = eventRaisedAt;
            existing.DeletedAt = existing.ActiveBookingCount == 0 ? eventRaisedAt : null;
            repositoryFactory.LocationBookingAccessRepository.Update(existing);
            changedFacts.Add(existing);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var changedFact in changedFacts.DistinctBy(item => new { item.CustomerId, item.LocationId }))
        {
            await cachedLocationBookingAccessService.RemoveByCustomerAndLocationAsync(
                changedFact.CustomerId,
                changedFact.LocationId,
                cancellationToken);
        }
    }

    private async Task SignalLocationBookingDerivedStateAsync(Event @event, CancellationToken cancellationToken)
    {
        foreach (var locationId in @event.Data.Booking.InvolvedLocationIds.RemoveInvalidIds().Distinct())
        {
            await temporalService.StartOrSignalWorkflowRecomputeLocationBookingDerivedStateAsync(
                new RecomputeLocationBookingDerivedStateInput(locationId),
                cancellationToken);
        }
    }

    private static IEnumerable<LocationBookingAccess> ToAccessFacts(Event @event)
    {
        var booking = @event.Data.Booking;
        var eventRaisedAt = @event.Metadata.Time.ToDateTimeOffset();
        var customerIds = booking.InvolvedCustomerIds.RemoveInvalidIds().Distinct();
        var locationIds = booking.InvolvedLocationIds.RemoveInvalidIds().Distinct();
        var organizationIds = booking.InvolvedOrganizationIds.RemoveInvalidIds().Distinct().DefaultIfEmpty(string.Empty);

        return from customerId in customerIds
            from locationId in locationIds
            from organizationId in organizationIds
            select new LocationBookingAccess
            {
                CustomerId = customerId,
                LocationId = locationId,
                OrganizationId = organizationId,
                ActiveBookingCount = 0,
                EventRaisedAt = eventRaisedAt
            };
    }
}
