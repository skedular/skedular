using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Temporalio.Client;

namespace Booking.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SignalWorkflowBookPrivateRecurringResourcesDeletedShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Signal_With_Correct_Workflow_Id_And_Args(
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        IUnitOfWork unitOfWork,
        string recurringBookingId,
        string expectedWorkflowId)
    {
        A.CallTo(() => workflowIdService.BookPrivateRecurringResources(recurringBookingId)).Returns(expectedWorkflowId);

        sut.SignalWorkflowBookPrivateRecurringResourcesDeleted(recurringBookingId, unitOfWork);

        A.CallTo(() => temporalSignalOutboxWorkflowExecutor.Signal(
            expectedWorkflowId,
            A<string>._,
            A<PrivateRecurringBookingDeletedArgs>.That.Matches(a => a.RecurringBookingId == recurringBookingId),
            A<WorkflowSignalOptions>._,
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
