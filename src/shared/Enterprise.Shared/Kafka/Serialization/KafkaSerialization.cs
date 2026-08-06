using Confluent.Kafka;

namespace Enterprise.Shared.Kafka.Serialization;

public static class KafkaSerialization
{
    /// <summary>
    ///     Magic byte that identifies a message with Confluent Platform framing.
    ///     Confluent serialization format version number; currently always 0.
    /// </summary>
    public const byte MagicByte = 0;

    /// <summary>
    ///     Size of the header that identifies the schema
    ///     https://docs.confluent.io/platform/current/schema-registry/serdes-develop/index.html#wire-format
    /// </summary>
    /// <remarks>
    ///     Magic Byte + Schema ID = 4
    /// </remarks>
    public const int HeaderByteCount = 1 + SchemaIdByteCount;

    /// <summary>
    ///     4 Bytes for storing the schema ID
    ///     https://docs.confluent.io/platform/current/schema-registry/serdes-develop/index.html#wire-format
    /// </summary>
    public const int SchemaIdByteCount = 4;

    public static readonly byte[] EmptySchemaHeader = [MagicByte, 0, 0, 0, 0];

    /// <summary>
    ///     Kafka Consumer and Producers natively access these (de)serializers
    ///     <see cref="Deserializers" /> and <seealso cref="Serializers" />
    /// </summary>
    private static readonly HashSet<Type> NativelyAvailableSerializers =
    [
        typeof(Null),
        typeof(Ignore),
        typeof(int),
        typeof(long),
        typeof(string),
        typeof(float),
        typeof(double),
        typeof(byte[]),
    ];

    public static bool CanSerializeNatively<T>() => NativelyAvailableSerializers.Contains(typeof(T));
}
