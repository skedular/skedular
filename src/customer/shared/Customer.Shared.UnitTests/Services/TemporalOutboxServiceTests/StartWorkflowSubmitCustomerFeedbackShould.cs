using Customer.Shared.Services;
using Customer.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Customer.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowSubmitCustomerFeedbackShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Workflow_With_Correct_Options(
        [Frozen]
        TemporalConfiguration temporalConfiguration,
        [Frozen]
        IWorkflowIdService workflowIdService,
        [Frozen]
        ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        SubmitCustomerFeedbackInput args,
        IUnitOfWork unitOfWork,
        string expectedId)
    {
        A.CallTo(() => workflowIdService.SubmitCustomerFeedback(args.CustomerFeedbackId)).Returns(expectedId);

        sut.StartWorkflowSubmitCustomerFeedback(args, unitOfWork);

        A.CallTo(() => temporalOutboxWorkflowExecutor.Execute<SubmitCustomerFeedback, SubmitCustomerFeedbackInput>(
            args,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicateFailedOnly),
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
