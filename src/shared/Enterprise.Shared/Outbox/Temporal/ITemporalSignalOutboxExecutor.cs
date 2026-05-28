using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Temporal;

public interface ITemporalSignalOutboxExecutor
{
    Task SignalAsync(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        CancellationToken cancellationToken);
}
