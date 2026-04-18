using Api.Shared.Events;
using Enterprise.Shared.Kafka.Consume.Extensions;

namespace Enterprise.Shared.Kafka.Consume;

public class TopicSetting<TEvent> where TEvent : IEvent, new()
{
    public TopicSetting(int retryCount, int delayBaseSeconds, string prefix)
    {
        var @event = new TEvent();

        Topic = @event.GetTopicName(prefix);
        RetryTopics = @event.GetRetryTopicSettings(prefix, retryCount, delayBaseSeconds).ToList();
        DeadLetterTopic = @event.GetDeadLetterTopicName(prefix);
    }

    public string Topic { get; }
    public IList<RetryTopicSetting> RetryTopics { get; }
    public string DeadLetterTopic { get; }
}
