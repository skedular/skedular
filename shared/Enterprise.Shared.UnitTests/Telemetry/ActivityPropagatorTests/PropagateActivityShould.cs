using Enterprise.Shared.Telemetry;
using OpenTelemetry.Context.Propagation;

namespace Enterprise.Shared.UnitTests.Telemetry.ActivityPropagatorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PropagateActivityShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Call_Inject(
        [Frozen] IPropagationContextGetter propagationContext,
        [Frozen] TextMapPropagator textMapPropagator,
        [Frozen] IPropagatorFunctionProvider<string> propagatorFunctionProvider,
        ActivityPropagator<string> activityAccessor)
    {
        var carrier = "my context string";

        var context = new PropagationContext();
        A.CallTo(() => propagationContext.GetPropagationContext()).Returns(context);

        activityAccessor.PropagateActivity(carrier);

        A.CallTo(() =>
                textMapPropagator.Inject(context, carrier, propagatorFunctionProvider.Inject))
            .MustHaveHappened();
    }


    [Theory]
    [AutoFakeItEasyData]
    public void Throw_ArgumentNullException_When_Null_Destination(
        ActivityPropagator<string> accessor) =>
        Assert.Throws<ArgumentNullException>(() => accessor.PropagateActivity(null!));


    [Theory]
    [AutoFakeItEasyData]
    public void HandleNullPropagationContext(
        [Frozen] TextMapPropagator textMapPropagator,
        [Frozen] IPropagationContextGetter getter,
        ActivityPropagator<string> accessor)
    {
        A.CallTo(() => getter.GetPropagationContext()).Returns(null);
        var destination = string.Empty;
        accessor.PropagateActivity(destination);

        A.CallTo(() =>
                textMapPropagator.Inject(A<PropagationContext>.Ignored, A<string>.Ignored,
                    A<Action<string, string, string>>.Ignored))
            .MustNotHaveHappened();
    }
}
