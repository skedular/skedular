using Confluent.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Kafka.Produce;

namespace Enterprise.Shared.Kafka;

/// <summary>
///     Pulls an instance of the typed producer from the factory.
/// </summary>
/// <remarks>
///     https://long2know.com/2018/02/net-core-open-generics-with-factory-pattern/
/// </remarks>
/// <typeparam name="TKey">Message Key type</typeparam>
/// <typeparam name="TValue">Message Value type</typeparam>
public class ProducerInstanceFromFactoryAdapter<TKey, TValue>(
    IProducerFactory factory,
    KafkaConfiguration kafkaConfiguration)
    : IProducer<TKey, TValue>
{
    private readonly IProducer<TKey, TValue> _producer = factory.Build<TKey, TValue>(kafkaConfiguration);
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public int AddBrokers(string brokers) => _producer.AddBrokers(brokers);

    public void SetSaslCredentials(string username, string password) =>
        _producer.SetSaslCredentials(username, password);

    public Handle Handle => _producer.Handle;

    public string Name => _producer.Name;

    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        string topic,
        Message<TKey, TValue> message,
        CancellationToken cancellationToken = new()) =>
        _producer.ProduceAsync(topic, message, cancellationToken);

    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        CancellationToken cancellationToken = new()) =>
        _producer.ProduceAsync(topicPartition, message, cancellationToken);

    public void Produce(
        string topic,
        Message<TKey, TValue> message,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null) =>
        _producer.Produce(topic, message, deliveryHandler);

    public void Produce(
        TopicPartition topicPartition,
        Message<TKey, TValue> message,
        Action<DeliveryReport<TKey, TValue>>? deliveryHandler = null) =>
        _producer.Produce(topicPartition, message, deliveryHandler);

    public int Poll(TimeSpan timeout) => _producer.Poll(timeout);

    public int Flush(TimeSpan timeout) => _producer.Flush(timeout);

    public void Flush(CancellationToken cancellationToken = new()) => _producer.Flush(cancellationToken);

    public void InitTransactions(TimeSpan timeout) => _producer.InitTransactions(timeout);

    public void BeginTransaction() => _producer.BeginTransaction();

    public void CommitTransaction(TimeSpan timeout) => _producer.CommitTransaction(timeout);

    public void CommitTransaction() => _producer.CommitTransaction();

    public void AbortTransaction(TimeSpan timeout) => _producer.AbortTransaction(timeout);

    public void AbortTransaction() => _producer.AbortTransaction();

    public void SendOffsetsToTransaction(
        IEnumerable<TopicPartitionOffset> offsets,
        IConsumerGroupMetadata groupMetadata,
        TimeSpan timeout) =>
        _producer.SendOffsetsToTransaction(offsets, groupMetadata, timeout);

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _producer.Dispose();
        }

        _disposed = true;
    }

    ~ProducerInstanceFromFactoryAdapter() => Dispose(false);
}
