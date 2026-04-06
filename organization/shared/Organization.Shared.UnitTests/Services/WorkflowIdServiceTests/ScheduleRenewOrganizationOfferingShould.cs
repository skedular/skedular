using Enterprise.Shared.Temporal;
using Organization.Shared.Services;

namespace Organization.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ScheduleRenewOrganizationOfferingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string organizationOfferingId,
        string expectedWorkflowId)
    {
        A.CallTo(() => temporalHelperService.ToId(organizationOfferingId)).Returns(expectedWorkflowId);

        sut.ScheduleRenewOrganizationOffering(organizationOfferingId).ShouldBe(expectedWorkflowId);
    }
}
