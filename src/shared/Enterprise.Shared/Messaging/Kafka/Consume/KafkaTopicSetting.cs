using Api.Shared.Events.Kafka;
using Enterprise.Shared.Messaging.Kafka.Consume.Extensions;

namespace Enterprise.Shared.Messaging.Kafka.Consume;

public class KafkaTopicSetting<TEvent> where TEvent : IKafkaEvent, new()
{
    public KafkaTopicSetting(int retryCount, int delayBaseSeconds, string prefix)
    {
        var @event = new TEvent();

        Topic = @event.GetTopicName(prefix);
        RetryTopics = [.. @event.GetRetryTopicSettings(prefix, retryCount, delayBaseSeconds)];
        DeadLetterTopic = @event.GetDeadLetterTopicName(prefix);
    }

    public string Topic { get; }
    public IList<KafkaRetryTopicSetting> RetryTopics { get; }
    public string DeadLetterTopic { get; }
}
