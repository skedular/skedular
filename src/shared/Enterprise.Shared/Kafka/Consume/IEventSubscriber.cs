using Api.Shared.Events;

namespace Enterprise.Shared.Kafka.Consume;

public interface IEventSubscriber<in TKey, in TEvent> where TKey : IEvent, new() where TEvent : IEvent, new()
{
    Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        TKey key,
        TEvent @event,
        CancellationToken cancellationToken);
}
