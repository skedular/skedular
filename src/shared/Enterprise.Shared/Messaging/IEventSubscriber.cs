namespace Enterprise.Shared.Messaging;

public interface IEventSubscriber<in TKey, in TEvent> where TKey : new() where TEvent : new()
{
    Task<EventSubscriberResult> HandleAsync(EventContext eventContext, TKey key, TEvent @event, CancellationToken cancellationToken);
}
