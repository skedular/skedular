using Confluent.Kafka;
using Enterprise.Shared.Kafka;
using Shouldly;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

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
