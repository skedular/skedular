using System.Text;
using Confluent.Kafka;
using Enterprise.Shared.Kafka;
using Shouldly;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Kafka.HeaderExtensionsTests;

public class HeaderGetShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void ReturnNullIHeaderDoesNotExist(string key, Headers headers)
    {
        var result = headers.Get(key);

        result.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ReturnValueIHeaderExist(string key, string value, Headers headers)
    {
        headers.Add(key, Encoding.UTF8.GetBytes(value));

        var result = headers.Get(key);

        result.ShouldNotBeNull();
        result.ShouldBe(value);
    }
}
