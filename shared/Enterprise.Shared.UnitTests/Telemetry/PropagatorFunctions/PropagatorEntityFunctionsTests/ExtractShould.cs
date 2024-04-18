using Enterprise.Shared.Telemetry;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using FluentAssertions;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagatorFunctions.PropagatorEntityFunctionsTests;

public class ExtractShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Extract_Values(PropagatorEntityFunctions functions, IPropagatorEntity entity)
    {
        entity.TraceContext = "{\"my field\":\"my value\"}";
        var extract = functions.Extract(entity, "my field");
        extract.Single().Should().Be("my value");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Empty_If_Not_Present(
        PropagatorEntityFunctions functions,
        IPropagatorEntity entity)
    {
        entity.TraceContext = "{}";
        var extract = functions.Extract(entity, "my other key");
        extract.Should().HaveCount(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Handle_Empty(PropagatorEntityFunctions functions, IPropagatorEntity entity)
    {
        entity.TraceContext = "";
        var extract = functions.Extract(entity, "my key").ToArray();
        extract.Should().HaveCount(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Handle_Malformed_Json(PropagatorEntityFunctions functions, IPropagatorEntity entity)
    {
        entity.TraceContext = "{\"my field\":\"my value";
        var extract = functions.Extract(entity, "my key").ToArray();
        extract.Should().HaveCount(0);
    }
}
