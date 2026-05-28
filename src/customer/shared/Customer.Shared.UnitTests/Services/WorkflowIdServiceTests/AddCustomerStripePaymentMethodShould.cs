using Customer.Shared.Services;
using Enterprise.Shared.Temporal;

namespace Customer.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddCustomerStripePaymentMethodShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string clientSecret,
        string expectedWorkflowId)
    {
        A.CallTo(() => temporalHelperService.ToId(clientSecret)).Returns(expectedWorkflowId);

        sut.AddCustomerStripePaymentMethod(clientSecret).ShouldBe(expectedWorkflowId);
    }
}
