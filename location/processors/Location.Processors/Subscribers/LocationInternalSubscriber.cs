using Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Key;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Event = Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Value.Type;

namespace Location.Processors.Subscribers;

public class LocationInternalSubscriber(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RecordDailyDeskCount:
                await HandleRecordDailyDeskCountEventAsync(@event, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleRecordDailyDeskCountEventAsync(Event @event, CancellationToken cancellationToken)
    {
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(@event.LocationId, cancellationToken);
        if (location is null)
        {
            return;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        if (await repositoryFactory.DailyDeskCountRecordingRepository
                .Query(new Specification<DailyDeskCountRecording>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && query.Location.Id == @event.LocationId &&
                        query.Date == startOfToday
                }).AnyAsync(cancellationToken))
        {
            return;
        }

        _ = repositoryFactory.DailyDeskCountRecordingRepository.Add(new DailyDeskCountRecording
        {
            Id = randomHelper.Generate(),
            Count = location.Desks.Count,
            Date = startOfToday,
            Location = location
        });

        location.DailyDeskCountLastRecordedAt = timeProvider.GetUtcNow();
        _ = repositoryFactory.LocationRepository.Update(location);

        await repositoryFactory.DailyDeskCountRecordingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
