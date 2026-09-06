using System.Reflection;

namespace Api.Shared.Events.Kafka;

public static class KafkaTopicAttributeHelper
{
    public static KafkaTopicAttribute GetKafkaTopicInfo<TEvent>() where TEvent : IKafkaEvent
    {
        var eventType = typeof(TEvent);
        var attribute = eventType.GetCustomAttribute<KafkaTopicAttribute>();
        return attribute ?? throw new ArgumentNullException(
            nameof(attribute),
            $"{eventType.FullName} does not have KafkaTopicAttribute implemented");
    }
}
