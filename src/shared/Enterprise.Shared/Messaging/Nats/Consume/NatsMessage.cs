using System.Globalization;

namespace Enterprise.Shared.Messaging.Nats.Consume;

public sealed class NatsMessage
{
    public const string TimestampHeader = "timestamp";
    public const string RetryTimestampHeader = "retry-timestamp";

    public required string Subject { get; init; }
    public required ReadOnlyMemory<byte> Payload { get; init; }
    public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public required Func<CancellationToken, ValueTask> AcknowledgeAsync { get; init; }

    public DateTimeOffset GetTimestamp() => GetHeaderTimestamp(TimestampHeader);
    public DateTimeOffset? GetRetryTimestamp() => TryGetHeaderTimestamp(RetryTimestampHeader);

    public void SetTimestamp(DateTimeOffset timestamp) => Headers[TimestampHeader] = timestamp.ToString("O", CultureInfo.InvariantCulture);
    public void SetRetryTimestamp(DateTimeOffset timestamp) => Headers[RetryTimestampHeader] = timestamp.ToString("O", CultureInfo.InvariantCulture);

    private DateTimeOffset GetHeaderTimestamp(string key) =>
        TryGetHeaderTimestamp(key) ?? throw new FormatException($"Missing or invalid NATS header '{key}'.");

    private DateTimeOffset? TryGetHeaderTimestamp(string key) =>
        Headers.TryGetValue(key, out var value) &&
        DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var timestamp)
            ? timestamp
            : null;
}
