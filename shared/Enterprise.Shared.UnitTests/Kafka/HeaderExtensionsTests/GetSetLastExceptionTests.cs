using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Kafka;
using Shouldly;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

public class GetSetLastExceptionTests
{
    [Theory]
    [AutoFakeItEasyData]
    public void ReturnNullIfValueNotSetInHeader(Message<string, byte[]> message)
    {
        var result = message.GetLastException();

        result.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ReturnValueSetInHeader(string exceptionMessage, Message<string, byte[]> message)
    {
        var exception = new Exception(exceptionMessage);

        message.SetLastException(exception);

        var result = message.GetLastException();

        result.ShouldNotBeNull();
        result.ShouldContain(exceptionMessage);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void SetHeader(string exceptionMessage, Message<string, byte[]> message)
    {
        var exception = new Exception(exceptionMessage);

        message.SetLastException(exception);

        var first = message.Headers.Single(header => header.Key == HeaderKeys.LastException);

        Encoding.UTF8.GetString(first.GetValueBytes()).ShouldContain(exceptionMessage);
    }
}
