using Enterprise.Shared.Events;

namespace Enterprise.Shared.UnitTests.Events.IEventTests;

[KafkaTopic(1, 1, 1, 1)]
public class TopicEvent : IEvent
{
    public string TopicName => "booking.created";
    public string RetryTopicNamePrefix => "booking.created.retry";
    public int RetryTopicCount => 2;
    public string DeadLetterTopicName => "booking.created.dead-letter";
}

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetTopicNameShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Prefix_with_environment_when_set(string environment)
    {
        environment = environment.Trim();
        if (string.IsNullOrWhiteSpace(environment))
        {
            environment = "local";
        }

        IEvent e = new TopicEvent();

        e.GetTopicName(environment).ShouldBe($"{environment}.booking.created");
    }

    [Fact]
    public void Return_bare_topic_name_when_environment_empty()
    {
        IEvent e = new TopicEvent();

        e.GetTopicName(string.Empty).ShouldBe("booking.created");
    }

    [Fact]
    public void Return_bare_retry_name_when_environment_empty()
    {
        IEvent e = new TopicEvent();

        e.GetRetryTopicName(string.Empty, 0).ShouldBe("booking.created.retry.0");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Prefix_retry_with_environment_when_set(string environment)
    {
        environment = environment.Trim();
        if (string.IsNullOrWhiteSpace(environment))
        {
            environment = "local";
        }

        IEvent e = new TopicEvent();

        e.GetRetryTopicName(environment, 1).ShouldBe($"{environment}.booking.created.retry.1");
    }

    [Fact]
    public void Return_dead_letter_name_when_environment_empty()
    {
        IEvent e = new TopicEvent();

        e.GetDeadLetterTopicName(string.Empty).ShouldBe("booking.created.dead-letter");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Prefix_dead_letter_with_environment_when_set(string environment)
    {
        environment = environment.Trim();
        if (string.IsNullOrWhiteSpace(environment))
        {
            environment = "local";
        }

        IEvent e = new TopicEvent();

        e.GetDeadLetterTopicName(environment).ShouldBe($"{environment}.booking.created.dead-letter");
    }

    [Fact]
    public void Return_null_correlation_id_by_default()
    {
        IEvent e = new TopicEvent();

        e.GetCorrelationId().ShouldBeNull();
    }
}
