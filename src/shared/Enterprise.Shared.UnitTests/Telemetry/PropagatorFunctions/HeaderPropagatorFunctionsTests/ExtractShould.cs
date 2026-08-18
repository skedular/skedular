using Confluent.Kafka;
using Enterprise.Shared.Kafka.Telemetry;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.HeaderPropagatorFunctionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ExtractShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Values(HeaderPropagatorFunctions functions)
    {
        var destination = new Headers
        {
            { "my key", [.. "my value"u8] },
        };
        var extract = functions.Extract(destination, "my key");
        extract.Single().ShouldBe("my value");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Nothing_When_Not_Present(HeaderPropagatorFunctions functions)
    {
        var destination = new Headers
        {
            { "my key", [.. "my value"u8] },
        };
        var extract = functions.Extract(destination, "a different key").ToList();
        extract.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Extract_All_Matching_Fields(HeaderPropagatorFunctions functions)
    {
        var destination = new Headers
        {
            { "my key", [.. "one"u8] },
            { "my key", [.. "two"u8] },
            { "my key", [.. "three"u8] },
        };
        var extract = functions.Extract(destination, "my key").ToArray();
        extract.Length.ShouldBe(3);
        extract[0].ShouldBe("one");
        extract[1].ShouldBe("two");
        extract[2].ShouldBe("three");
    }
}
