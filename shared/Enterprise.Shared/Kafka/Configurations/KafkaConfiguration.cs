using Confluent.Kafka;

namespace Enterprise.Shared.Kafka.Configurations;

public class KafkaConfiguration
{
    public const string Key = "Kafka";

    public string BootstrapServers { get; set; } = string.Empty;
    public SecurityProtocol? SecurityProtocol { get; set; }
    public SaslMechanism? SaslMechanism { get; set; }
    public bool UseSchemaRegistry { get; set; }
    public SchemaRegistryConfiguration? SchemaRegistry { get; set; }
    public int RetryTopicCount { get; set; } = 3;
    public string OutgoingTopicPrefix { get; set; } = string.Empty;
    public string IncomingTopicPrefix { get; set; } = string.Empty;
    public string? SaslUsername { get; set; }
    public string? SaslPassword { get; set; }
    public AutoOffsetReset? AutoOffsetReset { get; set; }

    /// <summary>
    ///     session.timeout.ms
    ///     The timeout used to detect worker failures. The worker sends periodic heartbeats to indicate its liveness to the
    ///     broker.
    ///     https://kafka.apache.org/documentation/#consumerconfigs_session.timeout.ms
    ///     Default:	45000 (45 seconds)
    /// </summary>
    public int? SessionTimeoutMs { get; set; }

    /// <summary>
    ///     heartbeat.interval.ms
    ///     The expected time between heartbeats to the group coordinator when using Kafka's group management facilities.
    ///     https://kafka.apache.org/documentation/#consumerconfigs_heartbeat.interval.ms
    ///     Default:	3000 (3 seconds)
    /// </summary>
    public int? HeartbeatIntervalMs { get; set; }

    /// <summary>
    ///     max.poll.interval.ms
    ///     Explanation: https://stackoverflow.com/a/39759329
    ///     The maximum delay between invocations of poll() when using consumer group management.
    ///     https://kafka.apache.org/documentation/#consumerconfigs_max.poll.interval.ms
    ///     Default: 300000 (5 minutes)
    /// </summary>
    /// <remarks>
    ///     If the processing thread dies, it takes max.poll.interval.ms to detect this.
    ///     However, if the whole consumer dies (and a dying processing thread most likely crashes the whole consumer including
    ///     the heartbeat thread),
    ///     it takes only session.timeout.ms to detect it.
    /// </remarks>
    public int? MaxPollIntervalMs { get; set; }

    /// <summary>
    ///     Maximum time the broker may wait to fill the Fetch response with fetch. min. bytes of messages. default: 500
    ///     importance: low
    /// </summary>
    public int? FetchWaitMaxMs { get; set; }

    /// <summary>
    ///     The maximum length of time (in milliseconds) before a cancellation request is acted on. Low values may result in
    ///     measurably higher CPU usage.
    ///     default: 100 range: 1 <= dotnet. cancellation. delay. max. ms <= 10000 importance: low
    /// </summary>
    public int? CancellationDelayMaxMs { get; set; }

    /// <summary>
    ///     Dictionary of extra consumer settings.
    ///     https://docs.confluent.io/platform/current/installation/configuration/consumer-configs.html#fetch-max-bytes
    /// </summary>
    /// <remarks>Settings in the dictionary are overriden by this classes parameters if set</remarks>
    /// <example>
    ///     <code> { "ConsumerSettings": { "fetch.max.wait.ms": 300 } </code>
    /// </example>
    public Dictionary<string, string> ConsumerSettings { get; set; } = new();

    /// <summary>
    ///     Dictionary of extra producer settings.
    ///     https://docs.confluent.io/platform/current/installation/configuration/consumer-configs.html#fetch-max-bytes
    /// </summary>
    /// <remarks>Settings in the dictionary are overriden by this classes parameters if set</remarks>
    /// <example>
    ///     <code> { "ProducerSettings": { "buffer.memory": 33554432 } </code>
    /// </example>
    public Dictionary<string, string> ProducerSettings { get; set; } = new();
}
