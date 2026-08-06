using System.Linq.Expressions;
using Customer.Shared.Services;
using Customer.Shared.Workflows;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Customer.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class StartWorkflowAddCustomerStripePaymentMethodAsyncShould
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
        WorkflowHandle<AddCustomerStripePaymentMethod, string> workflowHandle,
        TemporalService sut,
        AddCustomerStripePaymentMethodInput args,
        string expectedId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.AddCustomerStripePaymentMethod(args.ClientSecret)).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<AddCustomerStripePaymentMethod, Task<string>>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicateFailedOnly)))
            .Returns(workflowHandle);

        await sut.StartWorkflowAddCustomerStripePaymentMethodAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<AddCustomerStripePaymentMethod, Task<string>>>>._,
            A<WorkflowOptions>.That.Matches(options =>
                options.Id == expectedId &&
                options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicateFailedOnly))).MustHaveHappenedOnceExactly();
    }
}
