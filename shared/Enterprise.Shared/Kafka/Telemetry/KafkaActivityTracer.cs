using System.Diagnostics;
using Confluent.Kafka;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.Kafka.Telemetry;

public interface IKafkaActivityTracer
{
    Activity? CreateConsumeActivity<TKey, TValue>(ConsumeResult<TKey, TValue> consumeResult);
    Activity? CreateProduceActivity<TKey, TValue>(Message<TKey, TValue> message, string topic, int? partition = null);
}

/// <summary>
///     A helper class to start activities
/// </summary>
/// <remarks>
///     Refer to the message receiver example in the OpenTelemetry repo.
///     https://github.com/open-telemetry/opentelemetry-dotnet/blob/6b7f2dd77cf9d37260a853fcc95f7b77e296065d/examples/MicroserviceExample/Utils/Messaging/
///     JAVA Implementation:
///     https://github.com/open-telemetry/opentelemetry-java-instrumentation/blob/4820ec4855699cdcb6b76ce499ec629b116afbda/instrumentation/kafka-clients/kafka-clients-common/javaagent/main/java/io/opentelemetry/javaagent/instrumentation/kafka/KafkaConsumerAdditionalAttributesExtractor.java
/// </remarks>
public class KafkaActivityTracer(IActivityPropagator<Headers> propagator, IActivityGetter activityGetter, IKafkaActivityStarter activityStarter)
    : IKafkaActivityTracer
{
    /// <summary>
    ///     Name of the Kafka service
    /// </summary>
    private const string KafkaServiceName = "kafka";

    /// <summary>
    ///     Consume activities extract the context from the incoming Kafka message.
    ///     Nothing needs to be injected.
    /// </summary>
    /// <param name="consumeResult"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public Activity? CreateConsumeActivity<TKey, TValue>(ConsumeResult<TKey, TValue> consumeResult)
    {
        var propagationContext = propagator.GetActivityPropagationContext(consumeResult.Message.Headers);
        var activity = activityStarter.StartActivityFromContext(
            consumeResult.Topic,
            KafkaOperationType.Consume,
            propagationContext.ActivityContext, consumeResult.Partition.Value);
        if (activity is null)
        {
            return null;
        }

        SetKafkaTagsOnConsumeActivity(activity, consumeResult);
        activity.SetStatus(ActivityStatusCode.Ok);

        return activity;
    }

    /// <summary>
    ///     Producer activities are injected into the Kafka message.
    /// </summary>
    /// <param name="message">Message that will be sent</param>
    /// <param name="topic">Outgoing topic</param>
    /// <param name="partition">Topic partition. Null if not known</param>
    /// <typeparam name="TKey">Message Key Type</typeparam>
    /// <typeparam name="TValue">Message Value Type</typeparam>
    /// <returns></returns>
    public Activity? CreateProduceActivity<TKey, TValue>(Message<TKey, TValue> message, string topic, int? partition = null)
    {
        // there should be a parent activity, but if not just create an empty context
        var parentContext = activityGetter.GetCurrent()?.Context ?? new ActivityContext();
        var activity = activityStarter.StartActivityFromContext(topic, KafkaOperationType.Provide, parentContext, partition);
        if (activity is null)
        {
            return null;
        }

        propagator.PropagateActivity(message.Headers);
        SetKafkaTagsOnProduceActivity(activity, message);
        activity.SetStatus(ActivityStatusCode.Ok);

        return activity;
    }


    private static void SetKafkaTagsOnProduceActivity<TKey, TValue>(Activity activity, Message<TKey, TValue>? message = null)
    {
        activity.SetTag(SemanticConventions.PeerService, KafkaServiceName);

        if (!activity.IsAllDataRequested)
        {
            return;
        }

        if (message is not null)
        {
            activity.SetTag(SemanticConventions.MessagingKafkaMessageKey, message.Key);
        }

        activity.SetTag(SemanticConventions.MessagingTempDestination, false.ToString());
    }

    private static void SetKafkaTagsOnConsumeActivity<TKey, TValue>(Activity activity, ConsumeResult<TKey, TValue> result)
    {
        // Message should be set. If not, handle that before here.
        ArgumentNullException.ThrowIfNull(result.Message);

        activity.SetTag(SemanticConventions.ServiceName, KafkaServiceName);

        if (activity.IsAllDataRequested)
        {
            activity.SetTag(SemanticConventions.MessagingTempDestination, false.ToString());
            activity.SetTag(SemanticConventions.MessagingKafkaMessageKey, result.Message.Key);
        }
    }
}
