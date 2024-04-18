using Api.Shared.Events;
using Confluent.Kafka;

namespace Enterprise.Shared.Kafka.Consume;

public interface IEventSubscriber<in TKey, in TEvent> where TKey : IEvent, new() where TEvent : IEvent, new()
{
    Task HandleAsync(
        Headers headers,
        TKey key,
        TEvent @event,
        CancellationToken cancellationToken);
}
