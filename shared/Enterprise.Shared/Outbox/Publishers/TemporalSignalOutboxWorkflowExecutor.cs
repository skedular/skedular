using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Publishers;

public interface ITemporalSignalOutboxWorkflowExecutor<in TWorkflowSignalArgs> where TWorkflowSignalArgs : class
{
    void Signal(string workflowId, string signalType, TWorkflowSignalArgs args, WorkflowSignalOptions workflowSignalOptions, IUnitOfWork unitOfWork);
}

public class TemporalSignalOutboxWorkflowExecutor<TWorkflowSignalArgs>(
    IActivityAccessor activityAccessor,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
    : ITemporalSignalOutboxWorkflowExecutor<TWorkflowSignalArgs> where TWorkflowSignalArgs : class
{
    public void Signal(string workflowId, string signalType, TWorkflowSignalArgs args, WorkflowSignalOptions workflowSignalOptions,
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

            ArgumentNullException.ThrowIfNull(args);
            dbContext.TemporalSignalOutbox.Add(new TemporalSignalOutbox
            {
                Id = randomHelper.Generate(),
                WorkflowId = workflowId,
                SignalType = signalType,
                ExecutionArgs = JsonSerializer.Serialize(args),
                WorkflowSignalOptions = workflowSignalOptions,
                Timestamp = timeProvider.GetUtcNow()
            });
        }
    }
}
