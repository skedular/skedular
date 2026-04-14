using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Enterprise.Shared.Events;
using Enterprise.Shared.Kafka.Configurations;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka;

public interface IKafkaHelper
{
    Task CreateTopicForEventAsync<TEvent>() where TEvent : IEvent, new();
    Task RegisterKeyProtobufSchemaAsync<TEvent>() where TEvent : class, IEvent, IMessage<TEvent>, new();
    Task RegisterValueProtobufSchemaAsync<TEvent>() where TEvent : class, IEvent, IMessage<TEvent>, new();
}

public class KafkaHelper : IKafkaHelper
{
    private readonly AdminClientConfig _adminConfig;
    private readonly KafkaConfiguration _kafkaConfiguration;
    private readonly ILogger<KafkaHelper> _logger;
    private readonly ISchemaRegistryClient? _schemaRegistryClient;

    public KafkaHelper(
        KafkaConfiguration kafkaConfiguration,
        ILogger<KafkaHelper> logger,
        ISchemaRegistryClient? schemaRegistryClient = null)
    {
        _logger = logger;
        _kafkaConfiguration = kafkaConfiguration;
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);
        if (kafkaConfiguration.UseSchemaRegistry)
        {
            ArgumentNullException.ThrowIfNull(schemaRegistryClient);
        }

        _adminConfig = new AdminClientConfig { BootstrapServers = kafkaConfiguration.BootstrapServers };
        _schemaRegistryClient = schemaRegistryClient;
    }

    public async Task CreateTopicForEventAsync<TEvent>() where TEvent : IEvent, new()
    {
        _logger.LogDebug("Creating Kafka topics for event. EventType={EventType}", typeof(TEvent).FullName);
        var kafkaTopicInfo = KafkaTopicHelper.GetKafkaTopicInfo<TEvent>();
        var @event = new TEvent();
        var topic = @event.GetTopicName(_kafkaConfiguration.OutgoingTopicPrefix);
        var retryTopics = Enumerable.Range(0, @event.GetRetryTopicCount())
            .Select(idx => @event.GetRetryTopicName(_kafkaConfiguration.OutgoingTopicPrefix, idx)).ToArray();
        var deadLetterTopic = @event.GetDeadLetterTopicName(_kafkaConfiguration.OutgoingTopicPrefix);
        string[] topics = [topic, .. retryTopics, deadLetterTopic];
        using var adminClient = new AdminClientBuilder(_adminConfig).Build();
        var existingTopics = adminClient
            .GetMetadata(TimeSpan.FromSeconds(10)).Topics
            .Select(topicMetadata => topicMetadata)
            .ToList();
        var allTopicsToCreate = topics.Except(existingTopics.Select(topicMetadata => topicMetadata.Topic)).ToList();
        var allTopicsToUpdate = topics.Except(allTopicsToCreate).ToList();

        if (allTopicsToCreate.Count != 0)
        {
            _logger.LogInformation("Creating {NewTopicCount} new Kafka topic(s). EventType={EventType}",
                allTopicsToCreate.Count, typeof(TEvent).FullName);
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

        if (allTopicsToUpdate.Count != 0)
        {
            var newPartitionsSpecification = allTopicsToUpdate
                .Distinct()
                .Where(topicName =>
                {
                    var partitionCount = topicName == topic ? kafkaTopicInfo.TopicPartitionCount :
                        topicName == deadLetterTopic ? kafkaTopicInfo.DeadLetterTopicPartitionCount :
                        kafkaTopicInfo.RetryTopicPartitionCount;

                    var existingTopic = existingTopics.Single(item => item.Topic == topicName);

                    return existingTopic.Partitions.Count < partitionCount;
                })
                .Select(topicName =>
                {
                    var partitionCount = topicName == topic ? kafkaTopicInfo.TopicPartitionCount :
                        topicName == deadLetterTopic ? kafkaTopicInfo.DeadLetterTopicPartitionCount :
                        kafkaTopicInfo.RetryTopicPartitionCount;

                    return new PartitionsSpecification { Topic = topicName, IncreaseTo = partitionCount };
                })
                .ToList();

            if (newPartitionsSpecification.Count != 0)
            {
                _logger.LogInformation("Increasing partitions for {UpdateTopicCount} existing Kafka topic(s). EventType={EventType}",
                    newPartitionsSpecification.Count, typeof(TEvent).FullName);
                await adminClient.CreatePartitionsAsync(newPartitionsSpecification);
            }
        }

        _logger.LogInformation("Kafka topic setup complete. EventType={EventType}", typeof(TEvent).FullName);
    }

    public async Task RegisterKeyProtobufSchemaAsync<TEvent>() where TEvent : class, IEvent, IMessage<TEvent>, new()
    {
        if (!_kafkaConfiguration.UseSchemaRegistry)
        {
            _logger.LogDebug("Schema registry is disabled; skipping key schema registration. EventType={EventType}", typeof(TEvent).FullName);
            return;
        }

        _logger.LogInformation("Registering key protobuf schema. EventType={EventType}", typeof(TEvent).FullName);
        var (topic, retryTopics, deadLetterTopic) = GetTopicNames<TEvent>();

        await RegisterRuntimeSchemaAsync<TEvent>(MessageComponentType.Key, topic);
        await Task.WhenAll(retryTopics.Select(topicName => RegisterRuntimeSchemaAsync<TEvent>(MessageComponentType.Key, topicName)));
        await RegisterRuntimeSchemaAsync<TEvent>(MessageComponentType.Key, deadLetterTopic);
        _logger.LogInformation("Key protobuf schema registration complete. EventType={EventType}", typeof(TEvent).FullName);
    }

    public async Task RegisterValueProtobufSchemaAsync<TEvent>() where TEvent : class, IEvent, IMessage<TEvent>, new()
    {
        if (!_kafkaConfiguration.UseSchemaRegistry)
        {
            _logger.LogDebug("Schema registry is disabled; skipping value schema registration. EventType={EventType}",
                typeof(TEvent).FullName);
            return;
        }

        _logger.LogInformation("Registering value protobuf schema. EventType={EventType}", typeof(TEvent).FullName);
        var (topic, retryTopics, deadLetterTopic) = GetTopicNames<TEvent>();

        await RegisterRuntimeSchemaAsync<TEvent>(MessageComponentType.Value, topic);
        await Task.WhenAll(retryTopics.Select(topicName => RegisterRuntimeSchemaAsync<TEvent>(MessageComponentType.Value, topicName)));
        await RegisterRuntimeSchemaAsync<TEvent>(MessageComponentType.Value, deadLetterTopic);
        _logger.LogInformation("Value protobuf schema registration complete. EventType={EventType}", typeof(TEvent).FullName);
    }

    private async Task RegisterRuntimeSchemaAsync<TEvent>(MessageComponentType componentType, string topic)
        where TEvent : class, IEvent, IMessage<TEvent>, new()
    {
        ArgumentNullException.ThrowIfNull(_schemaRegistryClient);

        var serializer = new ProtobufSerializer<TEvent>(
            _schemaRegistryClient,
            new ProtobufSerializerConfig
            {
                AutoRegisterSchemas = true, NormalizeSchemas = true, SubjectNameStrategy = SubjectNameStrategy.Topic, SkipKnownTypes = true
            });

        await serializer.SerializeAsync(new TEvent(), new SerializationContext(componentType, topic));
        await _schemaRegistryClient.UpdateCompatibilityAsync(Compatibility.Forward, GetSubjectName(topic, componentType));
    }

    private (string Topic, string[] RetryTopics, string DeadLetterTopic) GetTopicNames<TEvent>() where TEvent : IEvent, new()
    {
        var @event = new TEvent();
        var topic = @event.GetTopicName(_kafkaConfiguration.OutgoingTopicPrefix);
        var retryTopics = Enumerable.Range(0, @event.GetRetryTopicCount())
            .Select(idx => @event.GetRetryTopicName(_kafkaConfiguration.OutgoingTopicPrefix, idx))
            .ToArray();

        return (topic, retryTopics, @event.GetDeadLetterTopicName(_kafkaConfiguration.OutgoingTopicPrefix));
    }

    private static string GetSubjectName(string topic, MessageComponentType componentType) =>
        componentType switch
        {
            MessageComponentType.Key => $"{topic}-key",
            MessageComponentType.Value => $"{topic}-value",
            _ => throw new ArgumentOutOfRangeException(nameof(componentType), componentType, null)
        };
}
