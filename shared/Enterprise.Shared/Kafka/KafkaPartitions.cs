using Confluent.Kafka;
using Serilog;

namespace Enterprise.Shared.Kafka;

public static class KafkaPartitions
{
    public static event EventHandler<TopicPartitionOffset[]>? PartitionsRevoked;
    public static event EventHandler<TopicPartition[]>? PartitionsAssigned;

    public static void SetPartitionChangeLogging<TKey, TValue>(
        this
            ConsumerBuilder<TKey, TValue> builder)
    {
        void TopicPartitionOffsetLogHandler(List<TopicPartitionOffset> changes, string state)
        {
            foreach (var topicPartitionOffset in changes.GroupBy(offset => offset.Topic))
            {
                Log.Information(
                    "{State} TopicPartitionOffset - Topic:{Topic} PartitionOffset {Partitions}",
                    state,
                    topicPartitionOffset.Key,
                    topicPartitionOffset.Select(offset =>
                        $"{offset.Partition.Value}::{offset.Offset.Value} "));
            }
        }

        void TopicPartitionLogHandler(
            List<TopicPartition> changes,
            string state)
        {
            foreach (var topicGroup in changes.GroupBy(partition => partition.Topic))
            {
                Log.Information(
                    "{State} TopicPartition - Topic:{Topic} Partitions:{Partitions}",
                    state, topicGroup.Key,
                    topicGroup.Select(partition => partition.Partition.Value).ToArray());
            }
        }

        builder.SetPartitionsLostHandler((_, list) => TopicPartitionOffsetLogHandler(list, "LOST"));

        builder.SetPartitionsAssignedHandler((consumer, list) =>
        {
            TopicPartitionLogHandler(list, "ASSIGNED");
            OnPartitionsAssigned(consumer, list.ToArray());
        });

        builder.SetPartitionsRevokedHandler((consumer, list) =>
        {
            TopicPartitionOffsetLogHandler(list, "REVOKED");
            OnPartitionRevoked(consumer, list.ToArray());
        });
    }

    private static void OnPartitionsAssigned<TKey, TValue>(
        IConsumer<TKey, TValue> consumer,
        TopicPartition[] toArray) =>
        PartitionsAssigned?.Invoke(consumer, toArray);

    private static void OnPartitionRevoked<TKey, TValue>(
        IConsumer<TKey, TValue> consumer,
        TopicPartitionOffset[] topicPartitionOffsets)
    {
        PartitionsRevoked?.Invoke(consumer, topicPartitionOffsets);
        // Hold the revocation a little to let some queued messages finish
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }
}
