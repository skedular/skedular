using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Publishers;

public interface ITemporalSignalOutboxWorkflowExecutor
{
    void Signal(string workflowId, string signalType, WorkflowSignalOptions workflowSignalOptions, IUnitOfWork unitOfWork);

    void Signal<TWorkflowSignalArgs>(
        string workflowId,
        string signalType,
        TWorkflowSignalArgs executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        IUnitOfWork unitOfWork) where TWorkflowSignalArgs : class;
}

public class TemporalSignalOutboxWorkflowExecutor(
    IActivityAccessor activityAccessor,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
    : ITemporalSignalOutboxWorkflowExecutor
{
    public void Signal(string workflowId, string signalType, WorkflowSignalOptions workflowSignalOptions, IUnitOfWork unitOfWork) =>
        SignalInternal(workflowId, signalType, null, workflowSignalOptions, unitOfWork);

    public void Signal<TWorkflowSignalArgs>(
        string workflowId,
        string signalType,
        TWorkflowSignalArgs executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        IUnitOfWork unitOfWork) where TWorkflowSignalArgs : class
    {
        ArgumentNullException.ThrowIfNull(executionArgs);
        SignalInternal(workflowId, signalType, JsonSerializer.Serialize(executionArgs), workflowSignalOptions, unitOfWork);
    }

    private void SignalInternal(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        IUnitOfWork unitOfWork)
    {
        using (activityAccessor
                   .GetActivitySource(TelemetryKeys.TemporalSignalActivitySourceName)
                   .StartActivity(TelemetryKeys.TemporalSignalEventSave))
        {
            dictionaryActivityPropagator.PropagateActivity(new Dictionary<string, string>());

            // ReSharper disable once SuspiciousTypeConversion.Global
            var dbContext = unitOfWork as ITemporalSignalOutboxStore;
            ArgumentNullException.ThrowIfNull(dbContext);

            dbContext.TemporalSignalOutbox.Add(new TemporalSignalOutbox
            {
                Id = randomHelper.Generate(),
                WorkflowId = workflowId,
                SignalType = signalType,
                ExecutionArgs = executionArgs,
                WorkflowSignalOptions = workflowSignalOptions,
                Timestamp = timeProvider.GetUtcNow()
            });
        }
    }
}
