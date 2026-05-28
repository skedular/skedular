using Enterprise.Shared.Temporal;
using Organization.Shared.Services;
using Organization.Shared.Workflows;

namespace Organization.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GenerateOrganizationDailyAnalyticsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string organizationId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.GenerateOrganizationDailyAnalyticsPrefix}-{organizationId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.GenerateOrganizationDailyAnalytics(organizationId).ShouldBe(expectedWorkflowId);
    }
}
