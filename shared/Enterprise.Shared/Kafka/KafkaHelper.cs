using Api.Shared.Events;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Confluent.SchemaRegistry;
using Enterprise.Shared.Kafka.Configurations;
using Schema = Confluent.SchemaRegistry.Schema;

namespace Enterprise.Shared.Kafka;

public interface IKafkaHelper
{
    Task CreateTopicForEventAsync<TEvent>() where TEvent : IEvent, new();
    Task RegisterKeyProtobufSchemaAsync<TEvent>() where TEvent : IEvent, new();
    Task RegisterValueProtobufSchemaAsync<TEvent>() where TEvent : IEvent, new();
}

public class KafkaHelper : IKafkaHelper
{
    private readonly AdminClientConfig _adminConfig;
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly ISchemaRegistryClient _schemaRegistryClient;

    public KafkaHelper(KafkaConfiguration kafkaConfiguration)
    {
        _kafkaConfiguration = kafkaConfiguration;
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);
        ArgumentNullException.ThrowIfNull(kafkaConfiguration.SchemaRegistry);

        _adminConfig = new AdminClientConfig { BootstrapServers = kafkaConfiguration.BootstrapServers };
        _schemaRegistryClient = new CachedSchemaRegistryClient(
            new SchemaRegistryConfig { Url = kafkaConfiguration.SchemaRegistry.Url });
    }

    public async Task CreateTopicForEventAsync<TEvent>() where TEvent : IEvent, new()
    {
        var kafkaTopicInfo = KafkaTopicHelper.GetKafkaTopicInfo<TEvent>();
        var @event = new TEvent();
        var topic = @event.GetTopicName(_kafkaConfiguration.OutgoingTopicPrefix);
        var retryTopics = Enumerable.Range(0, @event.GetRetryTopicCount())
            .Select(idx => @event.GetRetryTopicName(_kafkaConfiguration.OutgoingTopicPrefix, idx)).ToArray();
        var deadLetterTopic = @event.GetDeadLetterTopicName(_kafkaConfiguration.OutgoingTopicPrefix);
        string[] topics = [topic, ..retryTopics, deadLetterTopic];
        using var adminClient = new AdminClientBuilder(_adminConfig).Build();
        var existingTopics = adminClient
            .GetMetadata(TimeSpan.FromSeconds(10)).Topics.Select(topicMetadata => topicMetadata.Topic).ToList();
        var allTopicsToCreate = topics.Except(existingTopics).ToList();

        if (allTopicsToCreate.Count == 0)
        {
            return;
        }

        await adminClient.CreateTopicsAsync(
            allTopicsToCreate
                .Distinct()
                .Select(topicName =>
                {
                    var partitionCount = topicName == topic ? kafkaTopicInfo.TopicPartitionCount :
                        topicName == deadLetterTopic ? kafkaTopicInfo.DeadLetterTopicPartitionCount :
                        kafkaTopicInfo.RetryTopicPartitionCount;

                    return new TopicSpecification { Name = topicName, NumPartitions = partitionCount };
                })
                .ToList());
    }

    public async Task RegisterKeyProtobufSchemaAsync<TEvent>() where TEvent : IEvent, new()
    {
        var kafkaTopicInfo = KafkaTopicHelper.GetKafkaTopicInfo<TEvent>();
        var @event = new TEvent();
        var topic = @event.GetTopicName(_kafkaConfiguration.OutgoingTopicPrefix);
        var retryTopics = Enumerable.Range(0, @event.GetRetryTopicCount())
            .Select(idx => @event.GetRetryTopicName(_kafkaConfiguration.OutgoingTopicPrefix, idx)).ToArray();
        var deadLetterTopic = @event.GetDeadLetterTopicName(_kafkaConfiguration.OutgoingTopicPrefix);

        var tasks = new List<Task>
        {
            _schemaRegistryClient.RegisterSchemaAsync(
                SubjectNameStrategy.Topic.ConstructKeySubjectName(topic),
                new Schema(kafkaTopicInfo.ProtobufSchema, SchemaType.Protobuf))
        };

        tasks.AddRange(
            retryTopics.Select(topicName => _schemaRegistryClient.RegisterSchemaAsync(
                    SubjectNameStrategy.Topic.ConstructKeySubjectName(topicName),
                    new Schema(kafkaTopicInfo.ProtobufSchema, SchemaType.Protobuf)))
                .ToArray());

        tasks.Add(
            _schemaRegistryClient.RegisterSchemaAsync(
                SubjectNameStrategy.Topic.ConstructKeySubjectName(deadLetterTopic),
                new Schema(kafkaTopicInfo.ProtobufSchema, SchemaType.Protobuf)));

        await Task.WhenAll(tasks);
    }

    public async Task RegisterValueProtobufSchemaAsync<TEvent>() where TEvent : IEvent, new()
    {
        var kafkaTopicInfo = KafkaTopicHelper.GetKafkaTopicInfo<TEvent>();
        var @event = new TEvent();
        var topic = @event.GetTopicName(_kafkaConfiguration.OutgoingTopicPrefix);
        var retryTopics = Enumerable.Range(0, @event.GetRetryTopicCount())
            .Select(idx => @event.GetRetryTopicName(_kafkaConfiguration.OutgoingTopicPrefix, idx)).ToArray();
        var deadLetterTopic = @event.GetDeadLetterTopicName(_kafkaConfiguration.OutgoingTopicPrefix);

        var tasks = new List<Task>
        {
            _schemaRegistryClient.RegisterSchemaAsync(
                SubjectNameStrategy.Topic.ConstructValueSubjectName(topic),
                new Schema(kafkaTopicInfo.ProtobufSchema, SchemaType.Protobuf))
        };

        tasks.AddRange(
            retryTopics.Select(topicName => _schemaRegistryClient.RegisterSchemaAsync(
                    SubjectNameStrategy.Topic.ConstructValueSubjectName(topicName),
                    new Schema(kafkaTopicInfo.ProtobufSchema, SchemaType.Protobuf)))
                .ToArray());

        tasks.Add(
            _schemaRegistryClient.RegisterSchemaAsync(
                SubjectNameStrategy.Topic.ConstructValueSubjectName(deadLetterTopic),
                new Schema(kafkaTopicInfo.ProtobufSchema, SchemaType.Protobuf)));

        await Task.WhenAll(tasks);
    }
}
