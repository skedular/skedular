using Google.Protobuf;

namespace Enterprise.Shared.Messaging.Nats.Serialization;

public sealed class ProtobufNatsSerializer<T> : INatsSerializer<T> where T : class, IMessage<T>, new()
{
    public ReadOnlyMemory<byte> Serialize(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.ToByteArray();
    }

    public T Deserialize(ReadOnlyMemory<byte> payload)
    {
        var message = new T();
        message.MergeFrom(payload.ToArray());
        return message;
    }
}
