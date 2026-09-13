using Enterprise.Shared.Messaging.Nats.Produce;
using Google.Protobuf.WellKnownTypes;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Produce;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NatsPublisherShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Serialize_and_delegate_typed_message(
        [Frozen]
        INatsPublisher publisher,
        SerializedNatsPublisher<StringValue> sut,
        CancellationToken cancellationToken)
    {
        var message = new StringValue
        {
            Value = "hello",
        };

        await sut.PublishAsync("events.test", message, cancellationToken);

        A.CallTo(() => publisher.PublishAsync(
                "events.test",
                A<ReadOnlyMemory<byte>>._,
                A<IReadOnlyDictionary<string, string>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
