using Enterprise.Shared.Events;

namespace Enterprise.Shared.UnitTests.Events.KafkaTopicHelperTests;

[KafkaTopic(4, 3, 1, 1)]
public class SampleEvent : IEvent
{
    public string TopicName => "test.sample";
    public string RetryTopicNamePrefix => "test.sample.retry";
    public int RetryTopicCount => 3;
    public string DeadLetterTopicName => "test.sample.dead-letter";
}

public class EventWithoutAttribute : IEvent
{
    public string TopicName => "test.no-attr";
    public string RetryTopicNamePrefix => "test.no-attr.retry";
    public int RetryTopicCount => 1;
    public string DeadLetterTopicName => "test.no-attr.dead-letter";
}

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetKafkaTopicInfoShould
{
    [Fact]
    public void Return_attribute_when_present()
    {
        var info = KafkaTopicHelper.GetKafkaTopicInfo<SampleEvent>();

        info.ShouldNotBeNull();
        info.TopicPartitionCount.ShouldBe(4);
        info.RetryTopicCount.ShouldBe(3);
        info.RetryTopicPartitionCount.ShouldBe(1);
        info.DeadLetterTopicPartitionCount.ShouldBe(1);
    }

    [Fact]
    public void Throw_when_attribute_missing() =>
        Should.Throw<ArgumentNullException>(() => KafkaTopicHelper.GetKafkaTopicInfo<EventWithoutAttribute>());
}
