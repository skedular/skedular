using System.Linq.Expressions;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Services;
using Organization.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartOrSignalWorkflowRecomputeOrganizationBookingDerivedStateAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Workflow_With_Signal_With_Start_And_Correct_Options(
        [Frozen]
        TemporalConfiguration temporalConfiguration,
        [Frozen]
        ITemporalClient temporalClient,
        [Frozen]
        IWorkflowIdService workflowIdService,
        WorkflowHandle<RecomputeOrganizationBookingDerivedState> workflowHandle,
        TemporalService sut,
        RecomputeOrganizationBookingDerivedStateInput args,
        string expectedId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.RecomputeOrganizationBookingDerivedState(args.OrganizationId)).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<RecomputeOrganizationBookingDerivedState, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate)))
            .Returns(workflowHandle);

        await sut.StartOrSignalWorkflowRecomputeOrganizationBookingDerivedStateAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<RecomputeOrganizationBookingDerivedState, Task>>>._,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate))).MustHaveHappenedOnceExactly();
    }
}
