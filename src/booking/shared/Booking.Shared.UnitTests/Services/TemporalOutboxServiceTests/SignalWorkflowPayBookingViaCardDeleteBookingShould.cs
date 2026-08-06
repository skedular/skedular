using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Temporalio.Client;

namespace Booking.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SignalWorkflowPayBookingViaCardDeleteBookingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Signal_With_Correct_Workflow_Id(
        [Frozen]
        IWorkflowIdService workflowIdService,
        [Frozen]
        ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        IUnitOfWork unitOfWork,
        string bookingId,
        string expectedWorkflowId)
    {
        A.CallTo(() => workflowIdService.PayBookingViaCard(bookingId)).Returns(expectedWorkflowId);

        sut.SignalWorkflowPayBookingViaCardDeleteBooking(bookingId, unitOfWork);

        A.CallTo(() => temporalSignalOutboxWorkflowExecutor.Signal(
            expectedWorkflowId,
            A<string>._,
            A<WorkflowSignalOptions>._,
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
