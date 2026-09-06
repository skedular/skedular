namespace Enterprise.Shared.Messaging.Nats.Serialization;

public interface INatsSerializer<T>
{
    ReadOnlyMemory<byte> Serialize(T value);
    T Deserialize(ReadOnlyMemory<byte> payload);
}
