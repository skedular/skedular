using Enterprise.Shared.Messaging.Nats.Consume;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Consume;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NatsRetrySubjectSettingShould
{
    [Fact]
    public void Derive_ordered_subjects_and_exponential_delays()
    {
        var settings = NatsRetrySubjectSetting.Create("test.event", 3, 5);

        settings.Select(item => item.Subject).ShouldBe([
            "test.event.retry.0",
            "test.event.retry.1",
            "test.event.retry.2",
        ]);
        settings.Select(item => item.Delay).ShouldBe([
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(20),
        ]);
    }
}
