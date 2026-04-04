using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Enterprise.Shared.Kafka.Configurations;
using Google.Protobuf;

namespace Enterprise.Shared.Kafka.Serialization;

public class CustomProtobufSerializer<T> : ISerializer<T> where T : class, IMessage<T>, new()
{
    private readonly ProtobufSerializer<T>? _schemaRegistrySerializer;

    public CustomProtobufSerializer(KafkaConfiguration kafkaConfiguration, ISchemaRegistryClient? schemaRegistryClient = null)
    {
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        if (!kafkaConfiguration.UseSchemaRegistry)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(schemaRegistryClient);

        _schemaRegistrySerializer = new ProtobufSerializer<T>(
            schemaRegistryClient,
            new ProtobufSerializerConfig
            {
                AutoRegisterSchemas = kafkaConfiguration.SchemaRegistry?.AutoRegisterSchema ?? true,
                NormalizeSchemas = true,
                UseLatestVersion = kafkaConfiguration.SchemaRegistry?.UseLatestVersion ?? false,
                SubjectNameStrategy = SubjectNameStrategy.Topic,
                SkipKnownTypes = true
            });
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        if (_schemaRegistrySerializer is not null)
        {
#pragma warning disable VSTHRD002
            return _schemaRegistrySerializer.SerializeAsync(data, context).GetAwaiter().GetResult();
#pragma warning restore VSTHRD002
        }

        using var stream = new MemoryStream();

        stream.Write(KafkaSerialization.EmptySchemaHeader);
        data.WriteTo(stream);

        return stream.ToArray();
    }
}
