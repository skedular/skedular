using Confluent.Kafka;
using Enterprise.Shared.Telemetry;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Telemetry;

public class ProducerTelemetryDecorator<TKey, TValue>(
    IProducer<TKey, TValue> producer,
    IActivityAccessor activityAccessor,
    IKafkaActivityTracer tracer,
    ILogger<ProducerTelemetryDecorator<TKey, TValue>> logger)
    : IProducer<TKey, TValue>
{
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        string topic,
        Message<TKey, TValue> message,
        CancellationToken cancellationToken = new())
    {
        logger.LogDebug("Producing Kafka message with telemetry decorator. Topic={Topic}", topic);
        using (tracer.CreateProduceActivity(message, topic))
        {
            try
            {
                return await producer.ProduceAsync(topic, message, cancellationToken);
            }
            catch (Exception ex)
            {
                activityAccessor.AddException(ex);
                logger.LogWarning("Kafka async produce failed. Topic={Topic}, ExceptionType={ExceptionType}", topic, ex.GetType().Name);

                throw;
            }
        }
    }

    public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken cancellationToken = new())
    {
        logger.LogDebug("Producing Kafka message with telemetry decorator to partition. Topic={Topic}, Partition={Partition}",
            topicPartition.Topic,
            topicPartition.Partition.Value);
        using (tracer.CreateProduceActivity(message, topicPartition.Topic,
                   topicPartition.Partition.Value))
        {
            try
            {
                return await producer.ProduceAsync(topicPartition, message, cancellationToken);
            }
            catch (Exception ex)
            {
                activityAccessor.AddException(ex);
                logger.LogWarning("Kafka async produce to partition failed. Topic={Topic}, Partition={Partition}, ExceptionType={ExceptionType}",
                    topicPartition.Topic,
                    topicPartition.Partition.Value,
                    ex.GetType().Name);

                throw;
            }
        }
    }

    public void Produce(
        string topic,
        Message<TKey, TValue> message,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        logger.LogDebug("Producing Kafka message synchronously with telemetry decorator. Topic={Topic}", topic);
        using (tracer.CreateProduceActivity(message, topic))
        {
            try
            {
                producer.Produce(topic, message, deliveryHandler);
            }
            catch (Exception ex)
            {
                activityAccessor.AddException(ex);
                logger.LogWarning("Kafka synchronous produce failed. Topic={Topic}, ExceptionType={ExceptionType}", topic, ex.GetType().Name);

                throw;
            }
        }
    }

    public void Produce(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        logger.LogDebug("Producing Kafka message synchronously with telemetry decorator to partition. Topic={Topic}, Partition={Partition}",
            topicPartition.Topic,
            topicPartition.Partition.Value);
        using (tracer.CreateProduceActivity(message, topicPartition.Topic,
                   topicPartition.Partition.Value))
        {
            try
            {
                producer.Produce(topicPartition, message, deliveryHandler);
            }
            catch (Exception ex)
            {
                activityAccessor.AddException(ex);
                logger.LogWarning(
                    "Kafka synchronous produce to partition failed. Topic={Topic}, Partition={Partition}, ExceptionType={ExceptionType}",
                    topicPartition.Topic,
                    topicPartition.Partition.Value,
                    ex.GetType().Name);

                throw;
            }
        }
    }

    public int AddBrokers(string brokers) => producer.AddBrokers(brokers);

    public void SetSaslCredentials(string username, string password) => producer.SetSaslCredentials(username, password);

    public Handle Handle => producer.Handle;

    public string Name => producer.Name;

    public int Poll(TimeSpan timeout) => producer.Poll(timeout);

    public int Flush(TimeSpan timeout) => producer.Flush(timeout);

    public void Flush(CancellationToken cancellationToken = new()) => producer.Flush(cancellationToken);

    public void InitTransactions(TimeSpan timeout) => producer.InitTransactions(timeout);

    public void BeginTransaction() => producer.BeginTransaction();

    public void CommitTransaction(TimeSpan timeout) => producer.CommitTransaction(timeout);

    public void CommitTransaction() => producer.CommitTransaction();

    public void AbortTransaction(TimeSpan timeout) => producer.AbortTransaction(timeout);

    public void AbortTransaction() => producer.AbortTransaction();

    public void SendOffsetsToTransaction(
        IEnumerable<TopicPartitionOffset> offsets,
        IConsumerGroupMetadata groupMetadata,
        TimeSpan timeout) =>
        producer.SendOffsetsToTransaction(offsets, groupMetadata, timeout);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            logger.LogDebug("Disposing Kafka telemetry decorator producer. ProducerName={ProducerName}", producer.Name);
            producer.Dispose();
        }

        _disposed = true;
    }

    ~ProducerTelemetryDecorator() => Dispose(false);
}
