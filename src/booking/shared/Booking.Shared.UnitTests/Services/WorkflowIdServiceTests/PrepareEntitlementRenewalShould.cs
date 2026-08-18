using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;

namespace Booking.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class PrepareEntitlementRenewalShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen]
        ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string entitlementId,
        string expectedWorkflowId)
    {
        A.CallTo(() => temporalHelperService.ToId($"{Constants.PrepareEntitlementRenewalPrefix}-{entitlementId}"))
            .Returns(expectedWorkflowId);

        sut.PrepareEntitlementRenewal(entitlementId).ShouldBe(expectedWorkflowId);
    }
}
