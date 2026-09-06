using Api.Shared.Events.Kafka;

namespace Api.Shared.UnitTests.Events.IEventValidationTests;

[KafkaTopic(1, 1, 1, 1)]
file sealed class ValidKafkaEvent : IKafkaEvent
{
    public string TopicName => "booking.created";
    public string RetryTopicNamePrefix => "booking.created.retry";
    public int RetryTopicCount => 2;
    public string DeadLetterTopicName => "booking.created.dead-letter";
    public string? CorrelationId => null;
}

[KafkaTopic(1, 1, 1, 1)]
file sealed class CorrelationKafkaEvent : IKafkaEvent
{
    public string TopicName => "booking.created";
    public string RetryTopicNamePrefix => "booking.created.retry";
    public int RetryTopicCount => 2;
    public string DeadLetterTopicName => "booking.created.dead-letter";
    public string? CorrelationId => "test-correlation";
}

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetTopicNameValidationShould
{
    [Fact]
    public void Throw_on_invalid_characters_in_topic_name() =>
        // Event whose TopicName has spaces (invalid Kafka topic chars)
        Should.Throw<ArgumentException>(() =>
        {
            IKafkaEvent e = new ValidKafkaEvent();
            e.GetTopicName("invalid environment name with spaces");
        });

    [Fact]
    public void Return_retry_topic_count()
    {
        IKafkaEvent e = new ValidKafkaEvent();
        e.GetRetryTopicCount().ShouldBe(2);
    }

    [Fact]
    public void Return_custom_correlation_id()
    {
        IKafkaEvent e = new CorrelationKafkaEvent();
        e.GetCorrelationId().ShouldBe("test-correlation");
    }
}
