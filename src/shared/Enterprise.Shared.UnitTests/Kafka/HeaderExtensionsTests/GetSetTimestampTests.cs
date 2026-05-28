using Confluent.Kafka;
using Enterprise.Shared.Kafka;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetSetTimestampTests
{
    [Theory]
    [AutoFakeItEasyData]
    public void ReturnTimestampValue(Message<string, byte[]> message)
    {
        message.Timestamp = new Timestamp(DateTime.MinValue);

        var resul = message.GetTimestamp();

        resul.ShouldBe(DateTime.MinValue);

        message.SetTimestamp();

        resul = message.GetTimestamp();
        resul.ShouldNotBe(DateTime.MinValue);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void SetTimestamp(Message<string, byte[]> message)
    {
        message.Timestamp = new Timestamp(DateTime.MinValue);

        message.SetTimestamp();

        message.Timestamp.ShouldNotBe(new Timestamp(DateTime.MinValue));
    }
}
