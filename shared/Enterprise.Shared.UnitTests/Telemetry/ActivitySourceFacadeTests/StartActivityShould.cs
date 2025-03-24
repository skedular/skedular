using System.Diagnostics;
using Enterprise.Shared.Telemetry;
using FluentAssertions;
using FluentAssertions.Execution;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Telemetry.ActivitySourceFacadeTests;

public class StartActivityShould
{
    private const string TraceParent = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01";

    [Theory]
    [AutoFakeItEasyData]
    public void Start_Activity_Via_ActivitySource(
        string name,
        string activityName)
    {
        // set up listeners so the source will return activities
        var listener = new ActivityListener { ShouldListenTo = _ => true };

        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) =>
            ActivitySamplingResult.AllDataAndRecorded;

        ActivitySource.AddActivityListener(listener);

        var facade = new ActivitySourceFacade(name);
        var activityContext = ActivityContext.Parse(TraceParent, "");
        var startActivity = facade.StartActivity(activityName, ActivityKind.Producer, activityContext);

        startActivity.Should().NotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Start_Activity_With_Correct_Parameters(
        string name,
        string activityName,
        string tagKey,
        string tagValue)
    {
        // set up listeners so the source will return activities
        var listener = new ActivityListener { ShouldListenTo = _ => true };

        listener.Sample = (ref ActivityCreationOptions<ActivityContext> _) =>
            ActivitySamplingResult.AllDataAndRecorded;

        ActivitySource.AddActivityListener(listener);

        var facade = new ActivitySourceFacade(name);
        var activityContext =
            ActivityContext.Parse(TraceParent, "");
        var tags = new Dictionary<string, object?> { [tagKey] = tagValue };
        var activity = facade.StartActivity(activityName, ActivityKind.Producer, activityContext, tags);

        activity.Should().NotBeNull();

        using (new AssertionScope())
        {
            activity.DisplayName.Should().Be(activityName);
            activity.OperationName.Should().Be(activityName);

            activity.ParentId.Should().Be(TraceParent);
            activity.Tags.Should().ContainKey(tagKey);
            activity.Tags.Should().ContainValue(tagValue);
        }
    }
}
