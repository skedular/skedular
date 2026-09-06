using System.Globalization;
using Enterprise.Shared.Messaging.Nats.Consume;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NatsLogEvents = Enterprise.Shared.Messaging.Nats.Logging.NatsLogEvents;

namespace Enterprise.Shared.Messaging.Nats.Produce;

public interface INatsPublisherForwarder
{
    ValueTask PublishForwardedAsync(
        string subject,
        ReadOnlyMemory<byte> payload,
        IReadOnlyDictionary<string, string> headers,
        CancellationToken cancellationToken);
}

public class NatsPublisher(INatsConnection connection, TimeProvider timeProvider, ILogger<NatsPublisher> logger)
    : INatsPublisher, INatsPublisherForwarder
{
    private readonly NatsJSContext _jetStream = new(connection);

    public async ValueTask PublishAsync(
        string subject,
        ReadOnlyMemory<byte> payload,
        IReadOnlyDictionary<string, string>? headers,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        var natsHeaders = new NatsHeaders();

        if (headers is not null)
        {
            foreach (var (key, value) in headers)
            {
                natsHeaders[key] = value;
            }
        }

        natsHeaders[NatsMessage.TimestampHeader] = timeProvider.GetUtcNow().ToString("O", CultureInfo.InvariantCulture);

        try
        {
            var acknowledgment = await _jetStream.PublishAsync(subject, payload, headers: natsHeaders, cancellationToken: cancellationToken);
            acknowledgment.EnsureSuccess();
            NatsLogEvents.MessagePublished(logger, subject);
        }
        catch (Exception exception)
        {
            NatsLogEvents.MessagePublishFailed(logger, exception, subject);

            throw;
        }
    }

    public ValueTask PublishForwardedAsync(
        string subject,
        ReadOnlyMemory<byte> payload,
        IReadOnlyDictionary<string, string> headers,
        CancellationToken cancellationToken) =>
        PublishAsync(subject, payload, headers, cancellationToken);
}
