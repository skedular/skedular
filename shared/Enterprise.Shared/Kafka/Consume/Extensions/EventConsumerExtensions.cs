using Api.Shared.Events;

namespace Enterprise.Shared.Kafka.Consume.Extensions;

public static class EventConsumerExtensions
{
    extension<TEvent>(TEvent @event) where TEvent : IEvent
    {
        public IEnumerable<RetryTopicSetting> GetRetryTopicSettings(string prefix, int count, int delayBaseSeconds) =>
            Enumerable
                .Range(0, count)
                .Select(index =>
                    new RetryTopicSetting
                    {
                        Topic = @event.GetRetryTopicName(prefix, index), RetryDelaySeconds = Math.Pow(2, index) * delayBaseSeconds
                    });
    }
}
