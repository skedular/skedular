using Confluent.Kafka;
using Google.Protobuf;

namespace Enterprise.Shared.Kafka.Serialization;

public class CustomProtobufDeserializer<T> : IDeserializer<T> where T : class, IMessage<T>, new()
{
    private readonly MessageParser<T> _parser = new(() => new T());

    public T Deserialize(
        ReadOnlySpan<byte> bytes,
        bool isNull,
        SerializationContext context)
    {
        if (isNull)
        {
            throw new InvalidDataException("Expecting data not to be null");
        }

        var data = bytes.ToArray();
        var headerSize = data[0] == KafkaSerialization.MagicByte ? KafkaSerialization.HeaderByteCount : 0;

        return _parser.ParseFrom(data, headerSize, data.Length - headerSize);
    }
}
