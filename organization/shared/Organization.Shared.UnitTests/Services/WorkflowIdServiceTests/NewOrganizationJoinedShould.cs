using AutoFixture.Xunit3;
using Enterprise.Shared.Temporal;
using FakeItEasy;
using Organization.Shared.Services;
using Organization.Shared.Workflows;

namespace Organization.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NewOrganizationJoinedShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string organizationId,
        string organizationCustomDomain,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.NewOrganizationJoinedPrefix}-{organizationId}-{organizationCustomDomain}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.NewOrganizationJoined(organizationId, organizationCustomDomain).ShouldBe(expectedWorkflowId);
    }
}
