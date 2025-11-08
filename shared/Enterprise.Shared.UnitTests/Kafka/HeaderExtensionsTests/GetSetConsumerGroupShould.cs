using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Kafka;
using FluentAssertions;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

public class GetSetConsumerGroupShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void ReturnNullIfValueNotSetInHeader(
        Message<string, byte[]> message)
    {
        var result = message.GetConsumerGroup();

        result.Should().BeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ReturnValueSetInHeader(
        string consumerGroup,
        Message<string, byte[]> message)
    {
        message.SetConsumerGroup(consumerGroup);
        var result = message.GetConsumerGroup();

        result.Should().Be(consumerGroup);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void SetHeader(
        string consumerGroup,
        Message<string, byte[]> message)
    {
        message.SetConsumerGroup(consumerGroup);

        var first =
            message.Headers.Single(header => header.Key == HeaderKeys.ConsumerGroupMatch);

        Encoding.UTF8.GetString(first.GetValueBytes()).Should().Be(consumerGroup);
    }
}
