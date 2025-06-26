using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Publishers;

public interface ITemporalOutboxWorkflowExecutor<TWorkflow> where TWorkflow : class
{
    void Execute(WorkflowOptions workflowOptions, IUnitOfWork unitOfWork);
    void Execute<TWorkflowArgs>(TWorkflowArgs executionArgs, WorkflowOptions workflowOptions, IUnitOfWork unitOfWork) where TWorkflowArgs : class;
}

public class TemporalOutboxWorkflowExecutor<TWorkflow>(
    IActivityAccessor activityAccessor,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
    : ITemporalOutboxWorkflowExecutor<TWorkflow> where TWorkflow : class
{
    private readonly string _workflowType = typeof(TWorkflow).ToWorkflowType();

    public void Execute(WorkflowOptions workflowOptions, IUnitOfWork unitOfWork) => ExecuteInternal(null, workflowOptions, unitOfWork);

    public void Execute<TWorkflowArgs>(TWorkflowArgs executionArgs, WorkflowOptions workflowOptions, IUnitOfWork unitOfWork)
        where TWorkflowArgs : class
    {
        ArgumentNullException.ThrowIfNull(executionArgs);
        ExecuteInternal(JsonSerializer.Serialize(executionArgs), workflowOptions, unitOfWork);
    }

    private void ExecuteInternal(string? executionArgs, WorkflowOptions workflowOptions, IUnitOfWork unitOfWork)
    {
        using (activityAccessor.GetActivitySource(TelemetryKeys.TemporalActivitySourceName).StartActivity(TelemetryKeys.TemporalEventSave))
        {
            dictionaryActivityPropagator.PropagateActivity(new Dictionary<string, string>());

            // ReSharper disable once SuspiciousTypeConversion.Global
            var dbContext = unitOfWork as ITemporalOutboxStore;
            ArgumentNullException.ThrowIfNull(dbContext);

            dbContext.TemporalOutbox.Add(new TemporalOutbox
            {
                Id = randomHelper.Generate(),
                WorkflowType = _workflowType,
                ExecutionArgs = executionArgs,
                WorkflowOptions = workflowOptions,
                Timestamp = timeProvider.GetUtcNow()
            });
        }
    }
}
