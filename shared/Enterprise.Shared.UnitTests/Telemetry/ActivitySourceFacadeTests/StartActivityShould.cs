using System.Diagnostics;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.UnitTests.Telemetry.ActivitySourceFacadeTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartActivityShould
{
    private const string TraceParent = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";

    [Theory]
    [AutoFakeItEasyData]
    public void Start_Activity_Via_ActivitySource(string name, string activityName)
    {
        // set up listeners so the source will return activities
        var listener = new ActivityListener { ShouldListenTo = _ => true };

        listener.Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded;

        ActivitySource.AddActivityListener(listener);

        var facade = new ActivitySourceFacade(name);
        var activityContext = ActivityContext.Parse(TraceParent, "");
        var startActivity = facade.StartActivity(activityName, ActivityKind.Producer, activityContext);

        startActivity.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Start_Activity_With_Correct_Parameters(string name, string activityName, string tagKey, string tagValue)
    {
        // set up listeners so the source will return activities
        var listener = new ActivityListener { ShouldListenTo = _ => true };

        listener.Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded;

        ActivitySource.AddActivityListener(listener);

        var facade = new ActivitySourceFacade(name);
        var activityContext = ActivityContext.Parse(TraceParent, "");
        var tags = new Dictionary<string, object?> { [tagKey] = tagValue };
        var activity = facade.StartActivity(activityName, ActivityKind.Producer, activityContext, tags);

        activity.ShouldNotBeNull();

        activity.DisplayName.ShouldBe(activityName);
        activity.OperationName.ShouldBe(activityName);

        activity.ParentId.ShouldBe(TraceParent);
        activity.Tags.ShouldContain(item => item.Key == tagKey);
        activity.Tags.ShouldContain(item => item.Value == tagValue);
    }
}
