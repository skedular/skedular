using AutoFixture.Xunit3;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;
using FakeItEasy;

namespace Booking.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PayRecurringBookingViaBankTransferShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string recurringBookingId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.PaidRecurringBookingViaBankTransferPrefix}-{recurringBookingId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.PayRecurringBookingViaBankTransfer(recurringBookingId).ShouldBe(expectedWorkflowId);
    }
}
