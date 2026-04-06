using Enterprise.Shared.Temporal;
using Organization.Shared.Services;

namespace Organization.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddOrganizationStripePaymentMethodShould
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

        sut.AddOrganizationStripePaymentMethod(clientSecret).ShouldBe(expectedWorkflowId);
    }
}
