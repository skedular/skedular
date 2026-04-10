using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Temporalio.Client;

namespace Booking.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SignalWorkflowPayBookingViaCardSetPaymentStatusShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Signal_With_Correct_Workflow_Id_And_Args(
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        IUnitOfWork unitOfWork,
        SetPaymentStatusArgs executionArgs,
        string bookingId,
        string expectedWorkflowId)
    {
        A.CallTo(() => workflowIdService.PayBookingViaCard(bookingId)).Returns(expectedWorkflowId);

        sut.SignalWorkflowPayBookingViaCardSetPaymentStatus(bookingId, executionArgs, unitOfWork);

        A.CallTo(() => temporalSignalOutboxWorkflowExecutor.Signal(
            expectedWorkflowId,
            A<string>._,
            executionArgs,
            A<WorkflowSignalOptions>._,
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
