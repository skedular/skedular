using Enterprise.Shared.Messaging.Nats.Serialization;
using Google.Protobuf;

namespace Enterprise.Shared.Messaging.Nats.Produce;

public sealed class SerializedNatsPublisher<T>(INatsPublisher publisher, INatsSerializer<T> serializer) : INatsPublisher<T>
    where T : class, IMessage<T>, new()
{
    public ValueTask PublishAsync(string subject, T message, CancellationToken cancellationToken) =>
        publisher.PublishAsync(subject, serializer.Serialize(message), null, cancellationToken);
}
