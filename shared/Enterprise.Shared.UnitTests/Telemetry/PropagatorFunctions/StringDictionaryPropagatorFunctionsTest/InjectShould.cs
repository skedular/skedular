using Enterprise.Shared.Telemetry.PropagatorFunctions;
using FluentAssertions;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.
    StringDictionaryPropagatorFunctionsTest;

public class InjectShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Add_Field(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string>();
        functions.Inject(destination, "my field", "my value");
        destination["my field"].Should().Be("my value");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Overwrite_Previous_Field_Of_Same_Name(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string> { ["my field"] = "old value" };
        functions.Inject(destination, "my field", "my value");
        destination.Should().HaveCount(1);
        destination["my field"].Should().Be("my value");
    }
}
