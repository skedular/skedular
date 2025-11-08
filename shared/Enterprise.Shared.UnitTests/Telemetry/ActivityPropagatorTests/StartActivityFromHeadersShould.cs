using System.Diagnostics;
using AutoFixture.Xunit3;
using Enterprise.Shared.Telemetry;
using FakeItEasy;
using FluentAssertions;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Telemetry.ActivityPropagatorTests;

public class StartActivityShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Use_TextMapPropagator(
        [Frozen] IPropagatorFunctionProvider<string> functionProvider,
        [Frozen] TextMapPropagator propagator,
        ActivityPropagator<string> sut,
        string location)
    {
        sut.GetActivityPropagationContext(location);

        A.CallTo(() =>
                propagator.Extract(A<PropagationContext>.Ignored, location,
                    functionProvider.Extract))
            .MustHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_New_Context(
        [Frozen] IPropagatorFunctionProvider<string> functionProvider,
        [Frozen] TextMapPropagator propagator,
        ActivityPropagator<string> sut,
        string location)
    {
        // make the context unique
        var expected = new PropagationContext(
            ActivityContext.Parse("00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01", ""),
            new Baggage());

        A.CallTo(() =>
                propagator.Extract(A<PropagationContext>.Ignored, location,
                    functionProvider.Extract))
            .Returns(expected);

        var returned = sut.GetActivityPropagationContext(location);

        returned.Should().Be(expected);
    }
}
