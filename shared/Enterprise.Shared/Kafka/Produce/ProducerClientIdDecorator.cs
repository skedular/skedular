using Confluent.Kafka;

namespace Enterprise.Shared.Kafka.Produce;

/// <summary>
///     The Kafka producer decorator to add the client id into message header.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class ProducerClientIdDecorator<TKey, TValue>(IProducer<TKey, TValue> producer) : IProducer<TKey, TValue>
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
        SetProducerId(message);

        return await producer.ProduceAsync(topic, message, cancellationToken);
    }

    public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken cancellationToken = new())
    {
        SetProducerId(message);

        return await producer.ProduceAsync(topicPartition, message, cancellationToken);
    }

    public void Produce(
        string topic,
        Message<TKey, TValue> message,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        SetProducerId(message);
        producer.Produce(topic, message, deliveryHandler);
    }

    public void Produce(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null)
    {
        SetProducerId(message);
        producer.Produce(topicPartition, message, deliveryHandler);
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

    ~ProducerClientIdDecorator() => Dispose(false);

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

    private void SetProducerId(Message<TKey, TValue> message) => message.Set(HeaderKeys.ClientId, producer.Name);
}
