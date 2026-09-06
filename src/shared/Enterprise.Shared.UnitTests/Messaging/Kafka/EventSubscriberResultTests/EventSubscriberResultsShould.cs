using Enterprise.Shared.Messaging;

namespace Enterprise.Shared.UnitTests.Messaging.Kafka.EventSubscriberResultTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EventSubscriberResultsShould
{
    [Fact]
    public void Success_is_a_SuccessEventSubscriberResult() =>
        EventSubscriberResults.Success.ShouldBeOfType<SuccessEventSubscriberResult>();

    [Fact]
    public void Success_is_singleton_instance()
    {
        var a = EventSubscriberResults.Success;
        var b = EventSubscriberResults.Success;

        a.ShouldBeSameAs(b);
    }
}
