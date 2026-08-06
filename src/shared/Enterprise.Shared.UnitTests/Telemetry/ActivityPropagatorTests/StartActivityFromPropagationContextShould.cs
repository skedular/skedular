using System.Diagnostics;
using Enterprise.Shared.Telemetry;
using OpenTelemetry.Context.Propagation;

namespace Enterprise.Shared.UnitTests.Telemetry.ActivityPropagatorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartActivityFromPropagationContextShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Delegate_to_activity_source(
        [Frozen]
        IPropagatorFunctionProvider<string> functionProvider,
        [Frozen]
        TextMapPropagator propagator,
        IActivitySource activitySource,
        ActivityPropagator<string> sut,
        string location,
        string activityName)
    {
        var context = new PropagationContext();
        A.CallTo(() => propagator.Extract(A<PropagationContext>._, location, functionProvider.Extract)).Returns(context);

        sut.StartActivityFromPropagationContext(location, activitySource, activityName);

        A.CallTo(() => activitySource.StartActivity(
                activityName,
                ActivityKind.Internal,
                context.ActivityContext,
                A<IEnumerable<KeyValuePair<string, object?>>>._))
            .MustHaveHappenedOnceExactly();
    }
}
