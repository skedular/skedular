using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Messaging.Nats.Logging;

public static partial class NatsLogEvents
{
    [LoggerMessage(1000, LogLevel.Information, "NATS connection ready")]
    public static partial void ConnectionReady(ILogger logger);

    [LoggerMessage(1001, LogLevel.Information, "NATS JetStream resource provisioned. Stream={Stream} Subject={Subject}")]
    public static partial void ResourceProvisioned(ILogger logger, string stream, string subject);

    [LoggerMessage(1002, LogLevel.Information, "NATS message published. Subject={Subject}")]
    public static partial void MessagePublished(ILogger logger, string subject);

    [LoggerMessage(1003, LogLevel.Warning, "NATS message forwarding failed. Subject={Subject} NextSubject={NextSubject}")]
    public static partial void ForwardingFailed(ILogger logger, Exception exception, string subject, string nextSubject);

    [LoggerMessage(1010, LogLevel.Error, "NATS message publish failed. Subject={Subject}")]
    public static partial void MessagePublishFailed(ILogger logger, Exception exception, string subject);

    [LoggerMessage(1004, LogLevel.Information, "NATS consumer stopped. Subject={Subject}")]
    public static partial void ConsumerStopped(ILogger logger, string subject);

    [LoggerMessage(1005, LogLevel.Debug, "NATS message delivered. Subject={Subject}")]
    public static partial void MessageDelivered(ILogger logger, string subject);

    [LoggerMessage(1006, LogLevel.Debug, "NATS retry delay pending. Subject={Subject} Delay={Delay}")]
    public static partial void RetryDelayPending(ILogger logger, string subject, TimeSpan delay);

    [LoggerMessage(1007, LogLevel.Debug, "NATS message acknowledged. Subject={Subject}")]
    public static partial void MessageAcknowledged(ILogger logger, string subject);

    [LoggerMessage(1008, LogLevel.Information, "NATS connection reconnected")]
    public static partial void ConnectionReconnected(ILogger logger);

    [LoggerMessage(1009, LogLevel.Information, "NATS consumer shutdown. Subject={Subject}")]
    public static partial void ConsumerShutdown(ILogger logger, string subject);
}
