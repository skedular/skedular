using Location.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Location.Shared.Workflows;

public record DeprovisionHostLocationInput(string OrganizationId, string LocationId);

[Workflow]
public class DeprovisionHostLocation
{
    [WorkflowRun]
    public async Task ExecuteAsync(DeprovisionHostLocationInput args) =>
        await Workflow.ExecuteActivityAsync(
            (HostLocationProvisioning activity) => activity.DeprovisionAsync(args.OrganizationId, args.LocationId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 10, MaximumInterval = TimeSpan.FromMinutes(5) }
            });
}
