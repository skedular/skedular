using Customer.Shared.Services;
using Customer.Shared.Workflows;
using Temporalio.Client;

namespace Customer.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SignalAddCustomerStripePaymentMethodAndGetResultAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Get_Workflow_Handle_With_Correct_Id(
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        TemporalService sut,
        StripePaymentMethodEventState args,
        string clientSecret,
        string expectedWorkflowId,
        CancellationToken cancellationToken)
    {
        var workflowHandle = new WorkflowHandle<AddCustomerStripePaymentMethod>(temporalClient, expectedWorkflowId);

        A.CallTo(() => workflowIdService.AddCustomerStripePaymentMethod(clientSecret)).Returns(expectedWorkflowId);
        A.CallTo(() => temporalClient.GetWorkflowHandle<AddCustomerStripePaymentMethod>(expectedWorkflowId)).Returns(workflowHandle);

        await Should.ThrowAsync<NullReferenceException>(() =>
            sut.SignalAddCustomerStripePaymentMethodAndGetResultAsync(clientSecret, args, cancellationToken));

        A.CallTo(() => temporalClient.GetWorkflowHandle<AddCustomerStripePaymentMethod>(expectedWorkflowId)).MustHaveHappenedOnceExactly();
    }
}
