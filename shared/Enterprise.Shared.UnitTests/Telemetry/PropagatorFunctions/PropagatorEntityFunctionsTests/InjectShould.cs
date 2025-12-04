using Enterprise.Shared.Telemetry;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using Shouldly;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.PropagatorEntityFunctionsTests;

public class InjectShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Add_Field(PropagatorEntityFunctions functions, IPropagatorEntity entity)
    {
        entity.TraceContext = "{}";
        functions.Inject(entity, "my field", "my value");
        entity.TraceContext.ShouldBe("{\"my field\":\"my value\"}");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Overwrite_Previous_Field_Of_Same_Name(
        PropagatorEntityFunctions functions,
        IPropagatorEntity entity)
    {
        entity.TraceContext = "{\"my field\":\"my value\"}";
        functions.Inject(entity, "my field", "new value");
        entity.TraceContext.ShouldBe("{\"my field\":\"new value\"}");
    }


    [Theory]
    [AutoFakeItEasyData]
    public void Handle_Empty_String(PropagatorEntityFunctions functions, IPropagatorEntity entity)
    {
        entity.TraceContext = "";
        functions.Inject(entity, "my field", "new value");
        entity.TraceContext.ShouldBe("{\"my field\":\"new value\"}");
    }


    [Theory]
    [AutoFakeItEasyData]
    public void Handle_Malformed_Json(PropagatorEntityFunctions functions, IPropagatorEntity entity)
    {
        entity.TraceContext = "{\"my field\":\"my value\"";
        functions.Inject(entity, "my field", "new value");
        entity.TraceContext.ShouldBe("{\"my field\":\"new value\"}");
    }
}
