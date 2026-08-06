using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Enterprise.Shared.Kafka.Configurations;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Serialization;

public class CustomProtobufSerializer<T> : ISerializer<T> where T : class, IMessage<T>, new()
{
    private readonly ILogger<CustomProtobufSerializer<T>> _logger;
    private readonly ProtobufSerializer<T>? _schemaRegistrySerializer;

    public CustomProtobufSerializer(
        KafkaConfiguration kafkaConfiguration,
        ILogger<CustomProtobufSerializer<T>> logger,
        ISchemaRegistryClient? schemaRegistryClient = null)
    {
        _logger = logger;
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        if (!kafkaConfiguration.UseSchemaRegistry)
        {
            _logger.LogDebug("Schema registry is disabled for protobuf serializer. MessageType={MessageType}", typeof(T).FullName);
            return;
        }

        ArgumentNullException.ThrowIfNull(schemaRegistryClient);

        _logger.LogDebug("Schema registry is enabled for protobuf serializer. MessageType={MessageType}", typeof(T).FullName);

        _schemaRegistrySerializer = new ProtobufSerializer<T>(
            schemaRegistryClient,
            new ProtobufSerializerConfig
            {
                AutoRegisterSchemas = kafkaConfiguration.SchemaRegistry?.AutoRegisterSchema ?? true,
                NormalizeSchemas = true,
                UseLatestVersion = kafkaConfiguration.SchemaRegistry?.UseLatestVersion ?? false,
                SubjectNameStrategy = SubjectNameStrategy.Topic,
                SkipKnownTypes = true,
            });
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        _logger.LogDebug("Serializing protobuf Kafka message. MessageType={MessageType}, Topic={Topic}", typeof(T).FullName, context.Topic);
        if (_schemaRegistrySerializer is not null)
        {
#pragma warning disable VSTHRD002
            var schemaRegistryBytes = _schemaRegistrySerializer.SerializeAsync(data, context).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
            _logger.LogDebug("Serialized protobuf Kafka message with schema registry. MessageType={MessageType}, PayloadLength={PayloadLength}",
                typeof(T).FullName,
                schemaRegistryBytes.Length);
            return schemaRegistryBytes;
        }

        using var stream = new MemoryStream();

        stream.Write(KafkaSerialization.EmptySchemaHeader);
        data.WriteTo(stream);

        var payload = stream.ToArray();
        _logger.LogDebug("Serialized protobuf Kafka message without schema registry. MessageType={MessageType}, PayloadLength={PayloadLength}",
            typeof(T).FullName,
            payload.Length);
        return payload;
    }
}
