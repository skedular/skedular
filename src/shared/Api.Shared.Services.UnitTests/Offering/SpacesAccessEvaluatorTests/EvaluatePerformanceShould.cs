using System.Diagnostics;
using Api.Shared.Services.Offering;
using SharedOffering = Api.Shared.Services.Models.Offering;

namespace Api.Shared.Services.UnitTests.Offering.SpacesAccessEvaluatorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EvaluatePerformanceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Complete_Under_Five_Milliseconds_At_P95(SpacesAccessEvaluator sut)
    {
        var now = new DateTimeOffset(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
        var offering = new SharedOffering
        {
            Code = OfferingCode.SpacesFreeTierV1,
            Start = now,
            End = now.AddDays(14),
            SpacesProductEnabled = true,
            SpacesTrialStartedAt = now,
            SpacesTrialEndsAt = now.AddDays(14)
        };
        var elapsed = new double[1_000];

        for (var index = 0; index < elapsed.Length; index++)
        {
            var startedAt = Stopwatch.GetTimestamp();
            _ = sut.Evaluate(now, offering, SpacesAccessAction.CreateBookingInstance);
            elapsed[index] = Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds;
        }

        Array.Sort(elapsed);
        elapsed[949].ShouldBeLessThan(5d);
    }
}
