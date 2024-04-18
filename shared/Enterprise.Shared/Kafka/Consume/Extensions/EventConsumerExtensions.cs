using Api.Shared.Events;

namespace Enterprise.Shared.Kafka.Consume.Extensions;

public static class EventConsumerExtensions
{
    public static IEnumerable<RetryTopicSetting> GetRetryTopicSettings<TEvent>(
        this TEvent @event,
        string prefix,
        int count,
        int delayBaseSeconds) where TEvent : IEvent =>
        Enumerable.Range(0, count)
            .Select(index =>
                new RetryTopicSetting
                {
                    Topic = @event.GetRetryTopicName(prefix, index),
                    RetryDelaySeconds = Math.Pow(2, index) * delayBaseSeconds
                });
}
