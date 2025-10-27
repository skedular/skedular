using Location.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Location.Shared.Workflows.PrecomputeLocationProductRelationships;

public record ComputeOrganizationLocationsAndProductsRelationshipsInput(string OrganizationId);

[Workflow]
public class ComputeOrganizationLocationsAndProductsRelationships
{
    [WorkflowRun]
    public async Task ExecuteAsync(ComputeOrganizationLocationsAndProductsRelationshipsInput args) =>
        await Workflow.ExecuteActivityAsync(
            (LocationsProductsRelationships activity) => activity.ComputeLocationAndProductsRelationshipsAsync(args.OrganizationId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(30) }
            });
}
