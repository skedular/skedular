using System.Diagnostics;
using Enterprise.Shared.Telemetry;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Telemetry;

public interface IKafkaActivityStarter
{
    Activity? StartActivityFromContext(
        string topic,
        KafkaOperationType activityKind,
        ActivityContext parentContext = default,
        int? partition = null);
}

/// <summary>
///     Starts Kafka Activities
/// </summary>
public class KafkaActivityStarter(IActivityAccessor activityAccessor, ILogger<KafkaActivityStarter> logger) : IKafkaActivityStarter
{
    private const string MessagingSystem = "kafka";
    private const string DestinationKind = "topic";

    /// <summary>
    ///     Starts an activity from the provided context
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="activityKind"></param>
    /// <param name="parentContext">Parent Activity Context i.e the context that this activity is happening under</param>
    /// <param name="partition">If the partition is known, then this can be set</param>
    /// <returns>Activity if ActivitySource has listeners</returns>
    public Activity? StartActivityFromContext(
        string topic,
        KafkaOperationType activityKind,
        ActivityContext parentContext,
        int? partition = null)
    {
        logger.LogDebug("Starting Kafka activity from context. Topic={Topic}, OperationType={OperationType}, PartitionKnown={PartitionKnown}",
            topic,
            activityKind,
            partition.HasValue);

        var openTelemetryActivityKind = GetOpenTelemetryActivityKind(activityKind);

        var activityNameVerb = GetActivityVerb(activityKind);
        var activityName = $"{topic} {activityNameVerb}";
        var tags = BuildKafkaTags(topic, partition).ToList();

        var activitySource = activityAccessor.GetActivitySource(
            activityKind == KafkaOperationType.Consume
                ? TelemetryKeys.ConsumerActivitySourceName
                : TelemetryKeys.ProducerActivitySourceName);

        if (parentContext == default)
        {
            // Add a new context to prevent using the host context 
            parentContext = new ActivityContext(ActivityTraceId.CreateRandom(),
                ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded);
        }

        var activity = activitySource.StartActivity(
            activityName,
            openTelemetryActivityKind,
            parentContext,
            tags);
        logger.LogDebug("Kafka activity creation completed. Topic={Topic}, OperationType={OperationType}, ActivityCreated={ActivityCreated}",
            topic,
            activityKind,
            activity is not null);
        return activity;
    }

    /// <summary>
    ///     Gets the OpenTelemetry operation verb
    /// </summary>
    /// <param name="kafkaOperationType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static string GetActivityVerb(KafkaOperationType kafkaOperationType) => kafkaOperationType switch
    {
        KafkaOperationType.Provide => "send",
        KafkaOperationType.Consume => "receive",
        _ => throw new ArgumentOutOfRangeException(nameof(kafkaOperationType), kafkaOperationType,
            $"Unexpected value for {nameof(kafkaOperationType)}: {kafkaOperationType}. Update enum mapping or caller input."),
    };

    private static IDictionary<string, object?> BuildKafkaTags(string topic, int? partition)
    {
        var tags = new Dictionary<string, object?>
        {
            [SemanticConventions.MessagingSystem] = MessagingSystem,
            [SemanticConventions.MessagingDestination] = topic,
            [SemanticConventions.MessagingDestinationKind] = DestinationKind,
        };

        if (partition.HasValue)
        {
            tags[SemanticConventions.MessagingKafkaPartition] = partition.ToString()!;
        }

        return tags;
    }

    private static ActivityKind GetOpenTelemetryActivityKind(KafkaOperationType activityKind) =>
        activityKind switch
        {
            KafkaOperationType.Provide => ActivityKind.Producer,
            _ => ActivityKind.Consumer,
        };
}
