using Confluent.Kafka;
using Google.Protobuf;

namespace Enterprise.Shared.Kafka.Serialization;

public class CustomProtobufSerializer<T> : ISerializer<T> where T : class, IMessage<T>, new()
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        using var stream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(stream);

        stream.Write(KafkaSerialization.EmptySchemaHeader);
        data.WriteTo(stream);

        return stream.ToArray();
    }
}
