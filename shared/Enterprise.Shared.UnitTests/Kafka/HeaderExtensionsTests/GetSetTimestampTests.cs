using Confluent.Kafka;
using Enterprise.Shared.Kafka;
using FluentAssertions;
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

        resul.Should().Be(DateTime.MinValue);

        message.SetTimestamp();

        resul = message.GetTimestamp();
        resul.Should().NotBe(DateTime.MinValue);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void SetTimestamp(Message<string, byte[]> message)
    {
        message.Timestamp = new Timestamp(DateTime.MinValue);

        message.SetTimestamp();

        message.Timestamp.Should().NotBe(new Timestamp(DateTime.MinValue));
    }
}
