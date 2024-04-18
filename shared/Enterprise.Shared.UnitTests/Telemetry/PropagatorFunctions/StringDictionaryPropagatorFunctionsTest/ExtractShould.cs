using Enterprise.Shared.Telemetry.PropagatorFunctions;
using FluentAssertions;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.StringDictionaryPropagatorFunctionsTest;

public class ExtractShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Values(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string> { { "my key", "my value" } };
        var extract = functions.Extract(destination, "my key");
        extract.Single().Should().Be("my value");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Empty_If_Not_Present(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string> { { "my key", "my value" } };
        var extract = functions.Extract(destination, "my other key");
        extract.Should().HaveCount(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Field(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string> { { "my key", "one" }, { "my second key", "two" } };

        var extract = functions.Extract(destination, "my key").ToArray();
        extract[0].Should().Be("one");
    }
}
