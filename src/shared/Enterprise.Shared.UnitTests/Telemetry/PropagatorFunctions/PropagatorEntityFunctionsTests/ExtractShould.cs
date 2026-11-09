using Enterprise.Shared.Telemetry;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.PropagatorEntityFunctionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ExtractShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Values(PropagatorEntityFunctions functions, IPropagatorEntity entity)
    {
        entity.TraceContext = "{\"my field\":\"my value\"}";
        var extract = functions.Extract(entity, "my field");
        extract.Single().ShouldBe("my value");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Empty_If_Not_Present(
        PropagatorEntityFunctions functions,
        IPropagatorEntity entity)
    {
        entity.TraceContext = "{}";
        var extract = functions.Extract(entity, "my other key");
        extract.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Handle_Empty(PropagatorEntityFunctions functions, IPropagatorEntity entity)
    {
        entity.TraceContext = "";
        var extract = functions.Extract(entity, "my key").ToArray();
        extract.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Handle_Malformed_Json(PropagatorEntityFunctions functions, IPropagatorEntity entity)
    {
        entity.TraceContext = "{\"my field\":\"my value";
        var extract = functions.Extract(entity, "my key").ToArray();
        extract.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Treat_whitespace_context_as_absent_without_logging_a_warning(
        [Frozen]
        ILogger<PropagatorEntityFunctions> logger,
        PropagatorEntityFunctions functions,
        IPropagatorEntity entity,
        string fieldName)
    {
        entity.TraceContext = " \t\r\n";

        functions.Extract(entity, fieldName).ShouldBeEmpty();

        Fake.GetCalls(logger).Where(call => call.Method.Name == nameof(ILogger.Log)).ShouldBeEmpty();
    }
}
