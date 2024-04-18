using Confluent.Kafka;

namespace Enterprise.Shared.Kafka.Configurations;

public class KafkaConfiguration
{
    public const string Key = "Kafka";

    public string BootstrapServers { get; set; } = string.Empty;
    public int MaxMessageNumberToProcessAtAnyTime { get; set; } = 10;
    public SecurityProtocol? SecurityProtocol { get; set; }
    public SaslMechanism? SaslMechanism { get; set; }
    public bool UseSchemaRegistry { get; set; }
    public SchemaRegistryConfiguration? SchemaRegistry { get; set; }
    public string OutgoingTopicPrefix { get; set; } = string.Empty;
    public string IncomingTopicPrefix { get; set; } = string.Empty;
    public string SaslUsername { get; set; } = string.Empty;
    public string SaslPassword { get; set; } = string.Empty;
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
}
