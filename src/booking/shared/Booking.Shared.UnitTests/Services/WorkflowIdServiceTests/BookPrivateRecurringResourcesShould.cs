using Booking.Shared.Services;
using Enterprise.Shared.Temporal;

namespace Booking.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BookPrivateRecurringResourcesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string recurringBookingId,
        string expectedWorkflowId)
    {
        A.CallTo(() => temporalHelperService.ToId(recurringBookingId)).Returns(expectedWorkflowId);

        sut.BookPrivateRecurringResources(recurringBookingId).ShouldBe(expectedWorkflowId);
    }
}
