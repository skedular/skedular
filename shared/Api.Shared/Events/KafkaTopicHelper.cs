using System.Reflection;

namespace Api.Shared.Events;

public static class KafkaTopicHelper
{
    public static KafkaTopicAttribute GetKafkaTopicInfo<TEvent>() where TEvent : IEvent
    {
        var eventType = typeof(TEvent);
        var attribute = eventType.GetCustomAttribute<KafkaTopicAttribute>();
        if (attribute is null)
        {
            throw new ArgumentNullException($"{eventType.FullName} does not have KafkaTopicAttribute implemented",
                nameof(attribute));
        }

        return attribute;
    }
}
