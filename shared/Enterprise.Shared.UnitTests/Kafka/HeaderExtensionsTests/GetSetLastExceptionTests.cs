using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Kafka;
using FluentAssertions;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

public class GetSetLastExceptionTests
{
    [Theory]
    [AutoFakeItEasyData]
    public void ReturnNullIfValueNotSetInHeader(
        Message<string, byte[]> message)
    {
        var result = message.GetLastException();

        result.Should().BeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ReturnValueSetInHeader(
        string exceptionMessage,
        Message<string, byte[]> message)
    {
        var exception = new Exception(exceptionMessage);

        message.SetLastException(exception);

        var result = message.GetLastException();

        result.Should().Contain(exceptionMessage);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void SetHeader(
        string exceptionMessage,
        Message<string, byte[]> message)
    {
        var exception = new Exception(exceptionMessage);

        message.SetLastException(exception);

        var first =
            message.Headers.Single(header => header.Key == HeaderKeys.LastException);

        Encoding.UTF8.GetString(first.GetValueBytes()).Should().Contain(exceptionMessage);
    }
}
