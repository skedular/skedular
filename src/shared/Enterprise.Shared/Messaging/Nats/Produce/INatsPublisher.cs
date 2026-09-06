namespace Enterprise.Shared.Messaging.Nats.Produce;

public interface INatsPublisher
{
    ValueTask PublishAsync(
        string subject,
        ReadOnlyMemory<byte> payload,
        IReadOnlyDictionary<string, string>? headers,
        CancellationToken cancellationToken);
}
