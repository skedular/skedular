using Confluent.Kafka;

namespace Enterprise.Shared.Kafka.Consume;

/// <summary>
///     Thread context, primarily to pass ConsumeResult to the tests for confirmation
/// </summary>
/// <remarks>
///     Look at replacing the Headers parameter in the Subscriber with ConsumeResult
/// </remarks>
public static class ConsumeResultContext
{
    public static readonly ThreadLocal<ConsumeResult<byte[], byte[]>> Current = new();
}
