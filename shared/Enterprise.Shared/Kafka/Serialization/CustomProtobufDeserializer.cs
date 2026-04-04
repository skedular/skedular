using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Enterprise.Shared.Kafka.Configurations;
using Google.Protobuf;

namespace Enterprise.Shared.Kafka.Serialization;

public class CustomProtobufDeserializer<T> : IDeserializer<T> where T : class, IMessage<T>, new()
{
    private readonly MessageParser<T> _parser = new(() => new T());
    private readonly ProtobufDeserializer<T>? _schemaRegistryDeserializer;

    public CustomProtobufDeserializer(KafkaConfiguration kafkaConfiguration, ISchemaRegistryClient? schemaRegistryClient = null)
    {
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        if (!kafkaConfiguration.UseSchemaRegistry)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(schemaRegistryClient);

        _schemaRegistryDeserializer = new ProtobufDeserializer<T>(
            schemaRegistryClient,
            new ProtobufDeserializerConfig
            {
                UseLatestVersion = kafkaConfiguration.SchemaRegistry?.UseLatestVersion ?? false, SubjectNameStrategy = SubjectNameStrategy.Topic
            });
    }

    public T Deserialize(ReadOnlySpan<byte> bytes, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            throw new InvalidDataException("Expecting data not to be null");
        }

        var data = bytes.ToArray();

        if (_schemaRegistryDeserializer is not null)
        {
            try
            {
#pragma warning disable VSTHRD002
                return _schemaRegistryDeserializer.DeserializeAsync(data, isNull, context).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
            }
            catch (SchemaRegistryException) when (HasLegacyEmptySchemaHeader(data))
            {
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

    private static bool HasLegacyEmptySchemaHeader(byte[] data) =>
        data.Length >= KafkaSerialization.HeaderByteCount &&
        data[0] == KafkaSerialization.MagicByte &&
        data[1] == 0 &&
        data[2] == 0 &&
        data[3] == 0 &&
        data[4] == 0;
}
