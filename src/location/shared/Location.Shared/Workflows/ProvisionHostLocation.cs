using Location.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Location.Shared.Workflows;

public record ProvisionHostLocationInput(string OrganizationId, string LocationId, string LocationName);

[Workflow]
public class ProvisionHostLocation
{
    [WorkflowRun]
    public async Task ExecuteAsync(ProvisionHostLocationInput args) =>
        await Workflow.ExecuteActivityAsync(
            (HostLocationProvisioning activity) => activity.ProvisionAsync(args.OrganizationId, args.LocationId, args.LocationName),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 10, MaximumInterval = TimeSpan.FromMinutes(5) }
            });
}
