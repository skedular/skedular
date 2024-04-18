using Confluent.Kafka;
using Google.Protobuf;

namespace Enterprise.Shared.Kafka.Serialization;

public class CustomProtobufSerializer<T> : IAsyncSerializer<T> where T : class, IMessage<T>, new()
{
    public async Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        await using var stream = new MemoryStream();
        await using var binaryWriter = new BinaryWriter(stream);

        stream.Write(KafkaSerialization.EmptySchemaHeader);
        data.WriteTo(stream);

        return stream.ToArray();
    }
}
