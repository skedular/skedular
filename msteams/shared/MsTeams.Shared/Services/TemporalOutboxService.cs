using Enterprise.Shared.Outbox.Temporal;
using Temporalio.Client;

namespace MsTeams.Shared.Services;

public interface ITemporalOutboxService : ITemporalOutboxExecutor, ITemporalSignalOutboxExecutor;

public class TemporalOutboxService : ITemporalOutboxService
{
    public Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task SignalAsync(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
