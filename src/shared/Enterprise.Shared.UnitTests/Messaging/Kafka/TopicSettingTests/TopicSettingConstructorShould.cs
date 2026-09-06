using Api.Shared.Events.Kafka;
using Enterprise.Shared.Messaging.Kafka.Consume;

namespace Enterprise.Shared.UnitTests.Messaging.Kafka.TopicSettingTests;

[KafkaTopic(4, 3, 1, 1)]
file sealed class TestTopicKafkaEvent : IKafkaEvent
{
    public string TopicName => "test.topic";
    public string RetryTopicNamePrefix => "test.topic.retry";
    public int RetryTopicCount => 3;
    public string DeadLetterTopicName => "test.topic.dead-letter";
    public string? CorrelationId { get; }
}

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class TopicSettingConstructorShould
{
    [Fact]
    public void Build_topic_names_with_prefix()
    {
        var sut = new KafkaTopicSetting<TestTopicKafkaEvent>(3, 5, "dev");

        sut.Topic.ShouldBe("dev.test.topic");
        sut.RetryTopics.Count.ShouldBe(3);
        sut.DeadLetterTopic.ShouldBe("dev.test.topic.dead-letter");
    }

    [Fact]
    public void Build_retry_topic_settings()
    {
        var sut = new KafkaTopicSetting<TestTopicKafkaEvent>(3, 5, "dev");

        sut.RetryTopics[0].Topic.ShouldBe("dev.test.topic.retry.0");
        sut.RetryTopics[0].RetryDelaySeconds.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Build_topic_names_without_prefix()
    {
        var sut = new KafkaTopicSetting<TestTopicKafkaEvent>(3, 5, string.Empty);

        sut.Topic.ShouldBe("test.topic");
        sut.DeadLetterTopic.ShouldBe("test.topic.dead-letter");
    }
}
