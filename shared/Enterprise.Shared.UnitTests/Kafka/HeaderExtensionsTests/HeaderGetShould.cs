using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Kafka;
using FluentAssertions;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

public class HeaderGetShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void ReturnNullIHeaderDoesNotExist(string key, Headers headers)
    {
        var result = headers.Get(key);

        result.Should().BeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ReturnValueIHeaderExist(
        string key,
        string value,
        Headers headers)
    {
        headers.Add(key, Encoding.UTF8.GetBytes(value));

        var result = headers.Get(key);

        result.Should().NotBeNull();
        result.Should().Be(value);
    }
}
