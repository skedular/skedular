using Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1;
using Api.Shared.Events;

namespace Api.Shared.Clients.UnitTests.Events.Skedular.CustomerReadiness.V1;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CustomerReadinessMetadataShould
{
    [Fact]
    public void Report_correct_topic_name_on_key()
    {
        IEvent key = new Key();

        key.TopicName.ShouldBe("customer_readiness.event");
    }

    [Fact]
    public void Report_correct_topic_name_on_event()
    {
        IEvent e = new Event();

        e.TopicName.ShouldBe("customer_readiness.event");
    }

    [Fact]
    public void Report_correct_retry_topic_prefix_on_key()
    {
        IEvent key = new Key();

        key.RetryTopicNamePrefix.ShouldBe("customer_readiness.event.retry");
    }

    [Fact]
    public void Report_correct_dead_letter_topic_on_key()
    {
        IEvent key = new Key();

        key.DeadLetterTopicName.ShouldBe("customer_readiness.event.deadletter");
    }
}
