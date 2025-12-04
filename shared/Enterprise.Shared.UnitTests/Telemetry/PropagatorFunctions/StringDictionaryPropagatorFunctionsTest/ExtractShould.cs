using Enterprise.Shared.Telemetry.PropagatorFunctions;
using Shouldly;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.StringDictionaryPropagatorFunctionsTest;

public class ExtractShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Values(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string> { { "my key", "my value" } };
        var extract = functions.Extract(destination, "my key");
        extract.Single().ShouldBe("my value");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Empty_If_Not_Present(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string> { { "my key", "my value" } };
        var extract = functions.Extract(destination, "my other key");
        extract.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Field(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string> { { "my key", "one" }, { "my second key", "two" } };
        var extract = functions.Extract(destination, "my key").ToArray();
        extract[0].ShouldBe("one");
    }
}
