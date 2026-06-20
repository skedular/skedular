using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Services;
using Location.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Location.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartProvisionHostLocationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Workflow_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        ProvisionHostLocationInput args,
        IUnitOfWork unitOfWork,
        string expectedId)
    {
        A.CallTo(() => workflowIdService.ProvisionHostLocation(args.LocationId)).Returns(expectedId);

        sut.StartProvisionHostLocation(args, unitOfWork);

        A.CallTo(() => temporalOutboxWorkflowExecutor.Execute<ProvisionHostLocation, ProvisionHostLocationInput>(
            args,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicateFailedOnly),
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
