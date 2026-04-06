using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;

namespace Booking.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PayBookingViaCardShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string bookingId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.PaidViaCardPrefix}-{bookingId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.PayBookingViaCard(bookingId).ShouldBe(expectedWorkflowId);
    }
}
