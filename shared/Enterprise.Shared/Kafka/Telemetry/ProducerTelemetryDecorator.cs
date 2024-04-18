using Confluent.Kafka;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.Kafka.Telemetry;

public class ProducerTelemetryDecorator<TKey, TValue>(
    IProducer<TKey, TValue> producer,
    IActivityAccessor activityAccessor,
    IKafkaActivityTracer tracer)
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
        using (tracer.CreateProduceActivity(message, topic))
        {
            try
            {
                return await producer.ProduceAsync(topic, message, cancellationToken);
            }
            catch (Exception ex)
            {
                activityAccessor.RecordException(ex);

                throw;
            }
        }
    }

    public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken cancellationToken = new())
    {
        using (tracer.CreateProduceActivity(message, topicPartition.Topic,
                   topicPartition.Partition.Value))
        {
            try
            {
                return await producer.ProduceAsync(topicPartition, message, cancellationToken);
            }
            catch (Exception ex)
            {
                activityAccessor.RecordException(ex);

                throw;
            }
        }
    }

    public void Produce(
        string topic,
        Message<TKey, TValue> message,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        using (tracer.CreateProduceActivity(message, topic))
        {
            try
            {
                producer.Produce(topic, message, deliveryHandler);
            }
            catch (Exception ex)
            {
                activityAccessor.RecordException(ex);

                throw;
            }
        }
    }

    public void Produce(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        using (tracer.CreateProduceActivity(message, topicPartition.Topic,
                   topicPartition.Partition.Value))
        {
            try
            {
                producer.Produce(topicPartition, message, deliveryHandler);
            }
            catch (Exception ex)
            {
                activityAccessor.RecordException(ex);

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
            producer.Dispose();
        }

        _disposed = true;
    }

    ~ProducerTelemetryDecorator() => Dispose(false);
}
