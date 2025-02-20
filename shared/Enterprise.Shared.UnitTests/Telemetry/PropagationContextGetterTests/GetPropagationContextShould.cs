using System.Diagnostics;
using AutoFixture.Xunit3;
using Enterprise.Shared.Telemetry;
using FakeItEasy;
using FluentAssertions;
using OpenTelemetry.Context.Propagation;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagationContextGetterTests;

public class GetPropagationContextShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Propagation_Context(PropagationContextGetter getter)
    {
        var propagationContext = getter.GetPropagationContext();
        propagationContext.Should().Be(new PropagationContext());
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Null_On_Null_Activity([Frozen] IActivityGetter activityGetter, PropagationContextGetter getter)
    {
        A.CallTo(() => activityGetter.GetCurrent()).Returns(null);
        getter.GetPropagationContext().Should().BeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Current_ActivityContext([Frozen] IActivityGetter activityGetter, PropagationContextGetter getter)
    {
        var activity = new Activity("test");
        A.CallTo(() => activityGetter.GetCurrent()).Returns(activity);

        var propagationContext = getter.GetPropagationContext();
        propagationContext!.Value.ActivityContext.Should().Be(activity.Context);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Sets_Baggage([Frozen] IActivityGetter activityGetter, PropagationContextGetter getter)
    {
        var activity = new Activity("test");
        activity.AddBaggage("test", "entry");

        A.CallTo(() => activityGetter.GetCurrent()).Returns(activity);

        var propagationContext = getter.GetPropagationContext();
        var baggageResult = propagationContext!.Value.Baggage;
        baggageResult.GetBaggage("test").Should().Be("entry");
    }
}
