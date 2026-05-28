using System.Diagnostics;
using Enterprise.Shared.Telemetry;

namespace Enterprise.Shared.UnitTests.Telemetry.ActivityGetterTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
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
        current.ShouldBe(activity);
    }
}
