using System.Diagnostics;
using Enterprise.Shared.Telemetry;
using OpenTelemetry.Context.Propagation;

namespace Enterprise.Shared.UnitTests.Telemetry.PropagationContextGetterTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetPropagationContextShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Propagation_Context(PropagationContextGetter getter)
    {
        var propagationContext = getter.GetPropagationContext();
        propagationContext.ShouldBe(new PropagationContext());
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Null_On_Null_Activity([Frozen] IActivityGetter activityGetter, PropagationContextGetter getter)
    {
        A.CallTo(() => activityGetter.GetCurrent()).Returns(null);
        getter.GetPropagationContext().ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Current_ActivityContext([Frozen] IActivityGetter activityGetter, PropagationContextGetter getter)
    {
        var activity = new Activity("test");
        A.CallTo(() => activityGetter.GetCurrent()).Returns(activity);

        var propagationContext = getter.GetPropagationContext();
        propagationContext!.Value.ActivityContext.ShouldBe(activity.Context);
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
        baggageResult.GetBaggage("test").ShouldBe("entry");
    }
}
