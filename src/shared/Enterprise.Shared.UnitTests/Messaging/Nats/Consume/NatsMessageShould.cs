using Enterprise.Shared.Messaging.Nats.Consume;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Consume;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NatsMessageShould
{
    [Fact]
    public void Round_trip_timestamp_headers()
    {
        var timestamp = DateTimeOffset.Parse("2026-09-04T12:30:00.0000000+00:00");
        var message = new NatsMessage
        {
            Subject = "events.test",
            Payload = new byte[] { 1, 2, 3 },
            AcknowledgeAsync = _ => ValueTask.CompletedTask,
        };

        message.SetTimestamp(timestamp);
        message.SetRetryTimestamp(timestamp);

        message.GetTimestamp().ShouldBe(timestamp);
        message.GetRetryTimestamp().ShouldBe(timestamp);
    }
}
