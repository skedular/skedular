using Enterprise.Shared.Messaging.Nats.Configuration;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Provisioning;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NatsJetStreamProvisionerShould
{
    [Fact]
    public void Configure_one_in_flight_unlimited_delivery_consumer()
    {
        var definition = new NatsConsumerDefinition(
            "events",
            "events-main",
            "events.main",
            TimeSpan.FromSeconds(30),
            -1);

        var config = definition.ToClientConfig();

        config.DurableName.ShouldBe("events-main");
        config.FilterSubject.ShouldBe("events.main");
        config.MaxDeliver.ShouldBe(-1);
        config.MaxAckPending.ShouldBe(1);
    }

    [Fact]
    public void Configure_all_stream_subjects()
    {
        var definition = new NatsStreamDefinition("events", ["events.main", "events.retry.0", "events.dead"]);

        var config = definition.ToClientConfig();

        config.Name.ShouldBe("events");
        config.Subjects.ShouldBe(["events.main", "events.retry.0", "events.dead"]);
    }
}
