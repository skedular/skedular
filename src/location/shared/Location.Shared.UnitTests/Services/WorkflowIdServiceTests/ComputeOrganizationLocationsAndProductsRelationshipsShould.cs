using Enterprise.Shared.Temporal;
using Location.Shared.Services;
using Location.Shared.Workflows;

namespace Location.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ComputeOrganizationLocationsAndProductsRelationshipsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen]
        ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string organizationId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.ComputeLocationProductRelationshipsPrefix}-{organizationId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.ComputeOrganizationLocationsAndProductsRelationships(organizationId).ShouldBe(expectedWorkflowId);
    }
}
