using System.Text.Json.Nodes;
using Enterprise.Shared.Telemetry;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.PropagatorEntityFunctionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
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

    [Theory]
    [AutoFakeItEasyData]
    public void Treat_whitespace_context_as_empty_without_logging_a_warning(
        [Frozen]
        ILogger<PropagatorEntityFunctions> logger,
        PropagatorEntityFunctions functions,
        IPropagatorEntity entity,
        string fieldName,
        string fieldValue)
    {
        entity.TraceContext = " \t\r\n";

        functions.Inject(entity, fieldName, fieldValue);

        entity.TraceContext.ShouldBe(new JsonObject
        {
            [fieldName] = fieldValue,
        }.ToJsonString());
        Fake.GetCalls(logger).Where(call => call.Method.Name == nameof(ILogger.Log)).ShouldBeEmpty();
    }
}
