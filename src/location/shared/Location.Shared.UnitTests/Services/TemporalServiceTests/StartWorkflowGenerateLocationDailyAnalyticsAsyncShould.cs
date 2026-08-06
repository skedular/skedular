using System.Linq.Expressions;
using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Services;
using Location.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Location.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowGenerateLocationDailyAnalyticsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Workflow_With_Correct_Options(
        [Frozen]
        TemporalConfiguration temporalConfiguration,
        [Frozen]
        ITemporalClient temporalClient,
        [Frozen]
        IWorkflowIdService workflowIdService,
        WorkflowHandle<GenerateLocationDailyAnalytics> workflowHandle,
        TemporalService sut,
        GenerateLocationDailyAnalyticsInput args,
        string expectedId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.GenerateLocationDailyAnalytics(args.LocationId)).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<GenerateLocationDailyAnalytics, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                    options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting)))
            .Returns(workflowHandle);

        await sut.StartWorkflowGenerateLocationDailyAnalyticsAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<GenerateLocationDailyAnalytics, Task>>>._,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting))).MustHaveHappenedOnceExactly();
    }
}
