using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Microsoft.Extensions.Logging;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Temporal;

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
    TimeProvider timeProvider,
    ILogger<TemporalSignalOutboxWorkflowExecutor> logger)
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
        logger.LogDebug(
            "Queueing Temporal workflow signal in outbox. SignalType={SignalType}, HasExecutionArgs={HasExecutionArgs}",
            signalType,
            executionArgs is not null);

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

            logger.LogInformation("Temporal workflow signal queued in outbox successfully. SignalType={SignalType}", signalType);
        }
    }
}
