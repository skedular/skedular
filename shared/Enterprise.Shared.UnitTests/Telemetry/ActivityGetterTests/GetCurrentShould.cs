using System.Diagnostics;
using Enterprise.Shared.Telemetry;
using FluentAssertions;
using Testing.Shared;

namespace Enterprise.Shared.UnitTests.Telemetry.ActivityGetterTests;

public class GetCurrentShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Current_Activity(ActivityGetter getter)
    {
        var activity = new Activity("test");
        activity.Start(); // Activities are usually started by the activity source
        var current = getter.GetCurrent();
        activity.Stop();
        current.Should().Be(activity);
    }
}
