using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;

namespace Booking.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CleanupUnbookedResourceBookingSlotsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen]
        ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string expectedWorkflowId)
    {
        A.CallTo(() => temporalHelperService.ToId(Constants.CleanupUnbookedResourceBookingSlotsPrefix))
            .Returns(expectedWorkflowId);

        sut.CleanupUnbookedResourceBookingSlots().ShouldBe(expectedWorkflowId);
    }
}
