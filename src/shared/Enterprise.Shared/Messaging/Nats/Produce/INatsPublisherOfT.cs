using Google.Protobuf;

namespace Enterprise.Shared.Messaging.Nats.Produce;

public interface INatsPublisher<T> where T : class, IMessage<T>, new()
{
    ValueTask PublishAsync(string subject, T message, CancellationToken cancellationToken);
}
