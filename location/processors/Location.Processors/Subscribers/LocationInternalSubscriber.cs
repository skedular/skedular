using Api.Shared.Clients.Events.Skedular.LocationInternal.V1.Key;
using Api.Shared.Services.Models;
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
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RecordDailyDeskCount:
                await HandleRecordDailyDeskCountEventAsync(@event, cancellationToken);
                break;

            case Type.RecordDailyRoomCount:
                await HandleRecordDailyRoomCountEventAsync(@event, cancellationToken);
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
        if (await repositoryFactory.DailyDeskCountRecordingRepository.Query(
                new Specification<DailyDeskCountRecording>
                {
                    Criteria = query => !query.DeletedAt.HasValue && query.Location.Id == @event.LocationId && query.Date == startOfToday
                }).AnyAsync(cancellationToken))
        {
            return;
        }

        _ = repositoryFactory.DailyDeskCountRecordingRepository.Add(new DailyDeskCountRecording
        {
            Id = randomHelper.Generate(),
            Count = location.Resources
                .Count(item => item.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.Desk) && item.DeletedAt is null),
            Date = startOfToday,
            Location = location
        });

        location.DailyDeskCountLastRecordedAt = timeProvider.GetUtcNow();
        _ = repositoryFactory.LocationRepository.Update(location);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleRecordDailyRoomCountEventAsync(Event @event, CancellationToken cancellationToken)
    {
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(@event.LocationId, cancellationToken);
        if (location is null)
        {
            return;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        if (await repositoryFactory.DailyRoomCountRecordingRepository.Query(
                new Specification<DailyRoomCountRecording>
                {
                    Criteria = query => !query.DeletedAt.HasValue && query.Location.Id == @event.LocationId && query.Date == startOfToday
                }).AnyAsync(cancellationToken))
        {
            return;
        }

        _ = repositoryFactory.DailyRoomCountRecordingRepository.Add(new DailyRoomCountRecording
        {
            Id = randomHelper.Generate(), 
            Count = location.Resources
                .Count(item => item.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.Room) && item.DeletedAt is null),
            Date = startOfToday,
            Location = location
        });

        location.DailyRoomCountLastRecordedAt = timeProvider.GetUtcNow();
        _ = repositoryFactory.LocationRepository.Update(location);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
