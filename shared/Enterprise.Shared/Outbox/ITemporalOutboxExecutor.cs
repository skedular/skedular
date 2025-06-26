using Temporalio.Client;

namespace Enterprise.Shared.Outbox;

public interface ITemporalOutboxExecutor
{
    Task StartWorkflowAsync(string workflowType, string? executionArgs, WorkflowOptions workflowOptions, CancellationToken cancellationToken);
}
