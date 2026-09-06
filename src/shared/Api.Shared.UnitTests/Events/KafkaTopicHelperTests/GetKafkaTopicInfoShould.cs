using Api.Shared.Events.Kafka;

namespace Api.Shared.UnitTests.Events.KafkaTopicHelperTests;

[KafkaTopic(4, 3, 1, 1)]
public class SampleKafkaEvent : IKafkaEvent
{
    public string TopicName => "test.sample";
    public string RetryTopicNamePrefix => "test.sample.retry";
    public int RetryTopicCount => 3;
    public string DeadLetterTopicName => "test.sample.dead-letter";
    public string? CorrelationId => null;
}

public class KafkaEventWithoutAttribute : IKafkaEvent
{
    public string TopicName => "test.no-attr";
    public string RetryTopicNamePrefix => "test.no-attr.retry";
    public int RetryTopicCount => 1;
    public string DeadLetterTopicName => "test.no-attr.dead-letter";
    public string? CorrelationId => null;
}

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetKafkaTopicInfoShould
{
    [Fact]
    public void Return_attribute_when_present()
    {
        var info = KafkaTopicAttributeHelper.GetKafkaTopicInfo<SampleKafkaEvent>();

        info.ShouldNotBeNull();
        info.TopicPartitionCount.ShouldBe(4);
        info.RetryTopicCount.ShouldBe(3);
        info.RetryTopicPartitionCount.ShouldBe(1);
        info.DeadLetterTopicPartitionCount.ShouldBe(1);
    }

    [Fact]
    public void Throw_when_attribute_missing() =>
        Should.Throw<ArgumentNullException>(KafkaTopicAttributeHelper.GetKafkaTopicInfo<KafkaEventWithoutAttribute>);
}
