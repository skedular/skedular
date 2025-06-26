using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Publishers;

public interface ITemporalSignalOutboxWorkflowExecutor<TWorkflowSignal, in TWorkflowSignalArgs>
    where TWorkflowSignal : class where TWorkflowSignalArgs : class
{
    void Execute(string workflowId, TWorkflowSignalArgs args, WorkflowSignalOptions workflowSignalOptions, IUnitOfWork unitOfWork);
}

public class TemporalSignalOutboxWorkflowExecutor<TWorkflowSignal, TWorkflowSignalArgs>(
    IActivityAccessor activityAccessor,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
    : ITemporalSignalOutboxWorkflowExecutor<TWorkflowSignal, TWorkflowSignalArgs> where TWorkflowSignal : class where TWorkflowSignalArgs : class
{
    private readonly string _signalType = typeof(TWorkflowSignal).ToWorkflowSignalType();

    public void Execute(string workflowId, TWorkflowSignalArgs args, WorkflowSignalOptions workflowSignalOptions, IUnitOfWork unitOfWork)
    {
        using (activityAccessor.GetActivitySource(TelemetryKeys.TemporalSignalActivitySourceName).StartActivity(TelemetryKeys.TemporalSignalEventSave))
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
                SignalType = _signalType,
                ExecutionArgs = JsonSerializer.Serialize(args),
                WorkflowSignalOptions = workflowSignalOptions,
                Timestamp = timeProvider.GetUtcNow()
            });
        }
    }
}
