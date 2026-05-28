using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Organization.Shared.Services;
using Temporalio.Client;

namespace Organization.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SignalWorkflowScheduleRenewOrganizationOfferingCancelOfferingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Signal_With_Correct_Workflow_Id(
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        IUnitOfWork unitOfWork,
        string offeringId,
        string expectedWorkflowId)
    {
        A.CallTo(() => workflowIdService.ScheduleRenewOrganizationOffering(offeringId)).Returns(expectedWorkflowId);

        sut.SignalWorkflowScheduleRenewOrganizationOfferingCancelOffering(offeringId, unitOfWork);

        A.CallTo(() => temporalSignalOutboxWorkflowExecutor.Signal(
            expectedWorkflowId,
            A<string>._,
            A<WorkflowSignalOptions>._,
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
