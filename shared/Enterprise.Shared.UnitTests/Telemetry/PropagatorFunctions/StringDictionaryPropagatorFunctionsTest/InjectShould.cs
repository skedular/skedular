using Enterprise.Shared.Telemetry.PropagatorFunctions;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.
    StringDictionaryPropagatorFunctionsTest;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class InjectShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Add_Field(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string>();
        functions.Inject(destination, "my field", "my value");
        destination["my field"].ShouldBe("my value");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Overwrite_Previous_Field_Of_Same_Name(StringDictionaryPropagatorFunctions functions)
    {
        var destination = new Dictionary<string, string> { ["my field"] = "old value" };
        functions.Inject(destination, "my field", "my value");
        destination.Count.ShouldBe(1);
        destination["my field"].ShouldBe("my value");
    }
}
