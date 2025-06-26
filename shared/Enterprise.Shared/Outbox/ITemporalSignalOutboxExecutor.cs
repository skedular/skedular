using Temporalio.Client;

namespace Enterprise.Shared.Outbox;

public interface ITemporalSignalOutboxExecutor
{
    Task SignalAsync(string workflowId, string signalType, string? executionArgs, WorkflowSignalOptions workflowSignalOptions);
}
