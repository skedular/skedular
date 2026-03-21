using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Kafka;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetSetConsumerGroupShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void ReturnNullIfValueNotSetInHeader(
        Message<string, byte[]> message)
    {
        var result = message.GetConsumerGroup();

        result.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ReturnValueSetInHeader(
        string consumerGroup,
        Message<string, byte[]> message)
    {
        message.SetConsumerGroup(consumerGroup);
        var result = message.GetConsumerGroup();

        result.ShouldBe(consumerGroup);
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

        Encoding.UTF8.GetString(first.GetValueBytes()).ShouldBe(consumerGroup);
    }
}
