using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Kafka;
using Shouldly;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

public class GetSetRetryAttemptShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void ReturnNullIfValueNotSetInHeader(Message<string, byte[]> message)
    {
        var result = message.GetRetryAttempt();

        result.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ReturnValueSetInHeader(int retryAttempt, Message<string, byte[]> message)
    {
        message.SetRetryAttempt(retryAttempt);

        var result = message.GetRetryAttempt();

        result.ShouldBe(retryAttempt);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void SetHeader(int retryAttempt, Message<string, byte[]> message)
    {
        message.SetRetryAttempt(retryAttempt);

        var first = message.Headers.First(header => header.Key == HeaderKeys.RetryAttempt);

        Encoding.UTF8.GetString(first.GetValueBytes()).ShouldContain(retryAttempt.ToString());
    }
}
