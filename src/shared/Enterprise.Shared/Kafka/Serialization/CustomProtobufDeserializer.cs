using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Enterprise.Shared.Kafka.Configurations;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Kafka.Serialization;

public class CustomProtobufDeserializer<T> : IDeserializer<T> where T : class, IMessage<T>, new()
{
    private readonly ILogger<CustomProtobufDeserializer<T>> _logger;
    private readonly MessageParser<T> _parser = new(() => new T());
    private readonly ProtobufDeserializer<T>? _schemaRegistryDeserializer;

    public CustomProtobufDeserializer(
        KafkaConfiguration kafkaConfiguration,
        ILogger<CustomProtobufDeserializer<T>> logger,
        ISchemaRegistryClient? schemaRegistryClient = null)
    {
        _logger = logger;
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        if (!kafkaConfiguration.UseSchemaRegistry)
        {
            _logger.LogDebug("Schema registry is disabled for protobuf deserializer. MessageType={MessageType}", typeof(T).FullName);
            return;
        }

        ArgumentNullException.ThrowIfNull(schemaRegistryClient);

        _logger.LogDebug("Schema registry is enabled for protobuf deserializer. MessageType={MessageType}", typeof(T).FullName);

        _schemaRegistryDeserializer = new ProtobufDeserializer<T>(
            schemaRegistryClient,
            new ProtobufDeserializerConfig
            {
                UseLatestVersion = kafkaConfiguration.SchemaRegistry?.UseLatestVersion ?? false,
                SubjectNameStrategy = SubjectNameStrategy.Topic,
            });
    }

    public T Deserialize(ReadOnlySpan<byte> bytes, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            _logger.LogWarning("Received null Kafka payload during deserialisation. MessageType={MessageType}, Topic={Topic}",
                typeof(T).FullName, context.Topic);
            throw new InvalidDataException("Expecting data not to be null");
        }

        var data = bytes.ToArray();
        _logger.LogDebug("Deserialising Kafka message. MessageType={MessageType}, Topic={Topic}, PayloadLength={PayloadLength}",
            typeof(T).FullName, context.Topic, data.Length);

        if (_schemaRegistryDeserializer is not null)
        {
            try
            {
#pragma warning disable VSTHRD002
                var result = _schemaRegistryDeserializer.DeserializeAsync(data, isNull, context).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
                _logger.LogDebug("Deserialised Kafka message via schema registry. MessageType={MessageType}", typeof(T).FullName);
                return result;
            }
            catch (SchemaRegistryException) when (HasLegacyEmptySchemaHeader(data))
            {
                _logger.LogDebug("Schema registry deserialisation failed; falling back to legacy payload parsing. MessageType={MessageType}",
                    typeof(T).FullName);
                return ParseLegacyPayload(data);
            }
        }

        return ParseLegacyPayload(data);
    }

    private T ParseLegacyPayload(byte[] data)
    {
        var headerSize = data[0] == KafkaSerialization.MagicByte ? KafkaSerialization.HeaderByteCount : 0;

        return _parser.ParseFrom(data, headerSize, data.Length - headerSize);
    }

    private static bool HasLegacyEmptySchemaHeader(byte[] data) => data is [KafkaSerialization.MagicByte, 0, 0, 0, 0, ..];
}
