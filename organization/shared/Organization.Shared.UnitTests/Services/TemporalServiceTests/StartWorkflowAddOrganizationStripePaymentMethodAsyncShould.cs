using System.Linq.Expressions;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Services;
using Organization.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowAddOrganizationStripePaymentMethodAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Workflow_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        WorkflowHandle<AddOrganizationStripePaymentMethod, string> workflowHandle,
        TemporalService sut,
        AddOrganizationStripePaymentMethodInput args,
        string expectedId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.AddOrganizationStripePaymentMethod(args.ClientSecret)).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<AddOrganizationStripePaymentMethod, Task<string>>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicateFailedOnly)))
            .Returns(workflowHandle);

        await sut.StartWorkflowAddOrganizationStripePaymentMethodAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<AddOrganizationStripePaymentMethod, Task<string>>>>._,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicateFailedOnly))).MustHaveHappenedOnceExactly();
    }
}
