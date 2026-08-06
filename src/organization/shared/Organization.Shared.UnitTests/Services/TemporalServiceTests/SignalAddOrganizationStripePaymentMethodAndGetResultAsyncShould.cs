using Organization.Shared.Services;
using Organization.Shared.Workflows;
using Temporalio.Client;

namespace Organization.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SignalAddOrganizationStripePaymentMethodAndGetResultAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Get_Workflow_Handle_With_Correct_Id(
        [Frozen]
        ITemporalClient temporalClient,
        [Frozen]
        IWorkflowIdService workflowIdService,
        TemporalService sut,
        StripePaymentMethodEventState args,
        string clientSecret,
        string expectedWorkflowId,
        CancellationToken cancellationToken)
    {
        var workflowHandle = new WorkflowHandle<AddOrganizationStripePaymentMethod>(temporalClient, expectedWorkflowId);

        A.CallTo(() => workflowIdService.AddOrganizationStripePaymentMethod(clientSecret)).Returns(expectedWorkflowId);
        A.CallTo(() => temporalClient.GetWorkflowHandle<AddOrganizationStripePaymentMethod>(expectedWorkflowId)).Returns(workflowHandle);

        await Should.ThrowAsync<NullReferenceException>(() =>
            sut.SignalAddOrganizationStripePaymentMethodAndGetResultAsync(clientSecret, args, cancellationToken));

        A.CallTo(() => temporalClient.GetWorkflowHandle<AddOrganizationStripePaymentMethod>(expectedWorkflowId)).MustHaveHappenedOnceExactly();
    }
}
