using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Enterprise.Shared.Temporal;
using Microsoft.Extensions.Logging;
using Temporalio.Client;

namespace Enterprise.Shared.Outbox.Temporal;

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
    TimeProvider timeProvider,
    ILogger<TemporalOutboxWorkflowExecutor> logger)
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
        logger.LogDebug(
            "Queueing Temporal workflow in outbox. WorkflowType={WorkflowType}, HasExecutionArgs={HasExecutionArgs}",
            typeof(TWorkflow).FullName,
            executionArgs is not null);

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

            logger.LogInformation("Temporal workflow queued in outbox successfully. WorkflowType={WorkflowType}", typeof(TWorkflow).FullName);
        }
    }
}
