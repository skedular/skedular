using Enterprise.Shared.Messaging.Nats.Serialization;
using Google.Protobuf.WellKnownTypes;

namespace Enterprise.Shared.UnitTests.Messaging.Nats.Serialization;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProtobufNatsSerializerShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Round_trip_message(ProtobufNatsSerializer<StringValue> sut)
    {
        var source = new StringValue
        {
            Value = "message",
        };

        var result = sut.Deserialize(sut.Serialize(source));

        result.Value.ShouldBe(source.Value);
    }
}
