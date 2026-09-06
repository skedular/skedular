using Enterprise.Shared.Messaging.Nats.Consume;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Consume;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NatsReliableConsumeServiceMutationShould
{
    [Fact]
    public void Keep_retry_subject_settings_ordered_and_exponential()
    {
        var settings = NatsRetrySubjectSetting.Create("events.main", 3, 2);

        settings.Select(setting => setting.Subject).ShouldBe([
            "events.main.retry.0",
            "events.main.retry.1",
            "events.main.retry.2",
        ]);
        settings.Select(setting => setting.Delay).ShouldBe([
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(4),
            TimeSpan.FromSeconds(8),
        ]);
    }
}
