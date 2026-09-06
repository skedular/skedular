using Api.Shared.Events.Kafka;

namespace Enterprise.Shared.Messaging.Kafka.Consume.Extensions;

public static class KafkaEventConsumerExtensions
{
    extension<TEvent>(TEvent @event) where TEvent : IKafkaEvent
    {
        public IEnumerable<KafkaRetryTopicSetting> GetRetryTopicSettings(string prefix, int count, int delayBaseSeconds) =>
            Enumerable
                .Range(0, count)
                .Select(index =>
                    new KafkaRetryTopicSetting
                    {
                        Topic = @event.GetRetryTopicName(prefix, index),
                        RetryDelaySeconds = Math.Pow(2, index) * delayBaseSeconds,
                    });
    }
}
