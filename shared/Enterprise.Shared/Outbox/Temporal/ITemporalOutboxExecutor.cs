using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Temporal;

public interface ITemporalOutboxExecutor
{
    Task StartWorkflowAsync(string workflowType, string? executionArgs, WorkflowOptions workflowOptions, CancellationToken cancellationToken);
}
