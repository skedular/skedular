using Confluent.Kafka;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Configurations;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Produce;

public interface IPushToTopic<TKey>
{
    Task PushToTopicAsync(string topicName, Message<TKey, byte[]> message, CancellationToken cancellationToken);

    Task PushToExceptionTopicAsync(
        string topicName,
        Message<TKey, byte[]> message,
        int retryAttempt,
        Exception exception,
        CancellationToken cancellationToken);
}

public class PushToTopic<TKey>(
    IProducerFactory factory,
    ApplicationConfiguration applicationConfiguration,
    KafkaConfiguration kafkaConfiguration,
    ILogger<PushToTopic<TKey>> logger)
    : IPushToTopic<TKey>
{
    private readonly IProducer<TKey, byte[]> _producer = factory.Build<TKey, byte[]>(kafkaConfiguration);

    public async Task PushToTopicAsync(string topicName, Message<TKey, byte[]> message, CancellationToken cancellationToken)
    {
        try
        {
            message.SetTimestamp();
            await _producer.ProduceAsync(topicName, message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed to push message {Key} to {Topic}", message.Key, topicName);

            throw;
        }
    }

    public async Task PushToExceptionTopicAsync(
        string topicName,
        Message<TKey, byte[]> message,
        int retryAttempt,
        Exception exception,
        CancellationToken cancellationToken)
    {
        message.SetTimestamp();
        message.SetConsumerGroup(applicationConfiguration.GetSource());
        message.SetRetryAttempt(retryAttempt);
        message.SetLastException(exception);

        await PushToTopicAsync(topicName, message, cancellationToken);
    }
}
