using System.Reflection;

namespace Api.Shared.Events;

public static class KafkaTopicHelper
{
    public static KafkaTopicAttribute GetKafkaTopicInfo<TEvent>() where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        var attribute = eventType.GetCustomAttribute<KafkaTopicAttribute>();
        return attribute ?? throw new ArgumentNullException(
            nameof(attribute),
            $"{eventType.FullName} does not have KafkaTopicAttribute implemented");
    }
}
