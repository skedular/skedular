using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Publishers;

public interface ITemporalOutboxWorkflowExecutor<TWorkflow, in TWorkflowArgs> where TWorkflow : class where TWorkflowArgs : class
{
    void Execute(TWorkflowArgs args, WorkflowOptions workflowOptions, IUnitOfWork unitOfWork);
}

public class TemporalOutboxWorkflowExecutor<TWorkflow, TWorkflowArgs>(
    IActivityAccessor activityAccessor,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
    : ITemporalOutboxWorkflowExecutor<TWorkflow, TWorkflowArgs> where TWorkflow : class where TWorkflowArgs : class
{
    private readonly string _workflowType = typeof(TWorkflow).ToWorkflowType();

    public void Execute(TWorkflowArgs args, WorkflowOptions workflowOptions, IUnitOfWork unitOfWork)
    {
        using (activityAccessor.GetActivitySource(TelemetryKeys.TemporalActivitySourceName).StartActivity(TelemetryKeys.TemporalEventSave))
        {
            dictionaryActivityPropagator.PropagateActivity(new Dictionary<string, string>());

            // ReSharper disable once SuspiciousTypeConversion.Global
            var dbContext = unitOfWork as ITemporalOutboxStore;
            ArgumentNullException.ThrowIfNull(dbContext);

            ArgumentNullException.ThrowIfNull(args);
            dbContext.TemporalOutbox.Add(new TemporalOutbox
            {
                Id = randomHelper.Generate(),
                WorkflowType = _workflowType,
                ExecutionArgs = JsonSerializer.Serialize(args),
                WorkflowOptions = workflowOptions,
                Timestamp = timeProvider.GetUtcNow()
            });
        }
    }
}
