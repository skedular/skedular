using Enterprise.Shared.Messaging.Nats.Produce;
using Enterprise.Shared.Messaging.Nats.Serialization;
using Google.Protobuf.WellKnownTypes;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Produce;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NatsPublisherMutationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_publish_when_serialization_fails(
        [Frozen]
        INatsPublisher natsPublisher,
        [Frozen]
        INatsSerializer<StringValue> natsSerializer,
        SerializedNatsPublisher<StringValue> sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => natsSerializer.Serialize(A<StringValue>._)).Throws<InvalidOperationException>();

        await Should.ThrowAsync<InvalidOperationException>(() => sut.PublishAsync("events.test", new StringValue(), cancellationToken).AsTask());

        A.CallTo(() => natsPublisher.PublishAsync(
                A<string>._,
                A<ReadOnlyMemory<byte>>._,
                A<IReadOnlyDictionary<string, string>>._,
                A<CancellationToken>._))
            .MustNotHaveHappened();
    }
}
