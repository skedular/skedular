using Confluent.Kafka;

namespace Enterprise.Shared.Kafka.Telemetry;

public class ConsumerTelemetryDecorator<TKey, TValue>(
    IConsumer<TKey, TValue> consumer,
    IKafkaActivityTracer tracer)
    : IConsumer<TKey, TValue>
{
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public int AddBrokers(string brokers) => consumer.AddBrokers(brokers);

    public void SetSaslCredentials(string username, string password) => consumer.SetSaslCredentials(username, password);

    public Handle Handle => consumer.Handle;

    public string Name => consumer.Name;

    public ConsumeResult<TKey, TValue> Consume(int millisecondsTimeout)
    {
        var consumeResult = consumer.Consume(millisecondsTimeout);

        if (consumeResult is not null)
        {
            tracer.CreateConsumeActivity(consumeResult);
        }

        return consumeResult!;
    }

    public ConsumeResult<TKey, TValue> Consume(CancellationToken cancellationToken)
    {
        var consumeResult = consumer.Consume(cancellationToken);

        tracer.CreateConsumeActivity(consumeResult);

        return consumeResult;
    }

    public ConsumeResult<TKey, TValue> Consume(TimeSpan timeout)
    {
        var consumeResult = consumer.Consume(timeout);

        if (consumeResult is not null)
        {
            tracer.CreateConsumeActivity(consumeResult);
        }

        return consumeResult!;
    }

    public void Subscribe(IEnumerable<string> topics) => consumer.Subscribe(topics);

    public void Subscribe(string topic) => consumer.Subscribe(topic);

    public void Unsubscribe() => consumer.Unsubscribe();

    public void Assign(TopicPartition partition) => consumer.Assign(partition);

    public void Assign(TopicPartitionOffset partition) => consumer.Assign(partition);

    public void Assign(IEnumerable<TopicPartitionOffset> partitions) => consumer.Assign(partitions);

    public void Assign(IEnumerable<TopicPartition> partitions) => consumer.Assign(partitions);

    public void IncrementalAssign(IEnumerable<TopicPartitionOffset> partitions) =>
        consumer.IncrementalAssign(partitions);

    public void IncrementalAssign(IEnumerable<TopicPartition> partitions) => consumer.IncrementalAssign(partitions);

    public void IncrementalUnassign(IEnumerable<TopicPartition> partitions) => consumer.IncrementalUnassign(partitions);

    public void Unassign() => consumer.Unassign();

    public void StoreOffset(ConsumeResult<TKey, TValue> result) => consumer.StoreOffset(result);

    public void StoreOffset(TopicPartitionOffset offset) => consumer.StoreOffset(offset);

    public List<TopicPartitionOffset> Commit() => consumer.Commit();

    public void Commit(IEnumerable<TopicPartitionOffset> offsets) => consumer.Commit(offsets);

    public void Commit(ConsumeResult<TKey, TValue> result) => consumer.Commit(result);

    public void Seek(TopicPartitionOffset tpo) => consumer.Seek(tpo);

    public void Pause(IEnumerable<TopicPartition> partitions) => consumer.Pause(partitions);

    public void Resume(IEnumerable<TopicPartition> partitions) => consumer.Resume(partitions);

    public List<TopicPartitionOffset> Committed(TimeSpan timeout) => consumer.Committed(timeout);

    public List<TopicPartitionOffset> Committed(
        IEnumerable<TopicPartition> partitions,
        TimeSpan timeout) =>
        consumer.Committed(partitions, timeout);

    public Offset Position(TopicPartition partition) => consumer.Position(partition);

    public List<TopicPartitionOffset> OffsetsForTimes(
        IEnumerable<TopicPartitionTimestamp> timestampsToSearch,
        TimeSpan timeout) =>
        consumer.OffsetsForTimes(timestampsToSearch, timeout);

    public WatermarkOffsets GetWatermarkOffsets(TopicPartition topicPartition) =>
        consumer.GetWatermarkOffsets(topicPartition);

    public WatermarkOffsets QueryWatermarkOffsets(
        TopicPartition topicPartition,
        TimeSpan timeout) =>
        consumer.QueryWatermarkOffsets(topicPartition, timeout);

    public void Close() => consumer.Close();

    public string MemberId => consumer.MemberId;

    public List<TopicPartition> Assignment => consumer.Assignment;

    public List<string> Subscription => consumer.Subscription;

    public IConsumerGroupMetadata ConsumerGroupMetadata => consumer.ConsumerGroupMetadata;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            consumer.Dispose();
        }

        _disposed = true;
    }

    ~ConsumerTelemetryDecorator() => Dispose(false);
}
