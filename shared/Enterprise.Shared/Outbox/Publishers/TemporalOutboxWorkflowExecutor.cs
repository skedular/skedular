using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Database.Entities;
using Enterprise.Shared.Outbox.Telemetry;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Publishers;

public interface ITemporalOutboxWorkflowExecutor
{
    void Execute<TWorkflow>(WorkflowOptions workflowOptions, IUnitOfWork unitOfWork) where TWorkflow : class;

    void Execute<TWorkflow, TWorkflowArgs>(TWorkflowArgs executionArgs, WorkflowOptions workflowOptions, IUnitOfWork unitOfWork)
        where TWorkflow : class where TWorkflowArgs : class;
}

public class TemporalOutboxWorkflowExecutor(
    IActivityAccessor activityAccessor,
    IActivityPropagator<IDictionary<string, string>> dictionaryActivityPropagator,
    IRandomHelper randomHelper,
    TimeProvider timeProvider)
    : ITemporalOutboxWorkflowExecutor
{
    public void Execute<TWorkflow>(WorkflowOptions workflowOptions, IUnitOfWork unitOfWork) where TWorkflow : class =>
        ExecuteInternal<TWorkflow>(null, workflowOptions, unitOfWork);

    public void Execute<TWorkflow, TWorkflowArgs>(TWorkflowArgs executionArgs, WorkflowOptions workflowOptions, IUnitOfWork unitOfWork)
        where TWorkflow : class where TWorkflowArgs : class
    {
        ArgumentNullException.ThrowIfNull(executionArgs);
        ExecuteInternal<TWorkflow>(JsonSerializer.Serialize(executionArgs), workflowOptions, unitOfWork);
    }

    private void ExecuteInternal<TWorkflow>(string? executionArgs, WorkflowOptions workflowOptions, IUnitOfWork unitOfWork) where TWorkflow : class
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
                WorkflowType = typeof(TWorkflow).ToWorkflowType(),
                ExecutionArgs = executionArgs,
                WorkflowOptions = workflowOptions,
                Timestamp = timeProvider.GetUtcNow()
            });
        }
    }
}
