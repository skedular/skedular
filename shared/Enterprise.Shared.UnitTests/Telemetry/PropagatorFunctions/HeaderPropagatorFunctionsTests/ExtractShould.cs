using Confluent.Kafka;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using FluentAssertions;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.HeaderPropagatorFunctionsTests;

public class ExtractShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Values(HeaderPropagatorFunctions functions)
    {
        var destination = new Headers { { "my key", "my value"u8.ToArray() } };
        var extract = functions.Extract(destination, "my key");
        extract.Single().Should().Be("my value");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Nothing_When_Not_Present(HeaderPropagatorFunctions functions)
    {
        var destination = new Headers { { "my key", "my value"u8.ToArray() } };
        var extract = functions.Extract(destination, "a different key");
        extract.Should().HaveCount(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Extract_All_Matching_Fields(HeaderPropagatorFunctions functions)
    {
        var destination = new Headers { { "my key", "one"u8.ToArray() }, { "my key", "two"u8.ToArray() }, { "my key", "three"u8.ToArray() } };
        var extract = functions.Extract(destination, "my key").ToArray();
        extract.Should().HaveCount(3);
        extract[0].Should().Be("one");
        extract[1].Should().Be("two");
        extract[2].Should().Be("three");
    }
}
