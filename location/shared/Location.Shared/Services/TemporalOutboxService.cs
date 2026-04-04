using System.Text.Json;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Location.Shared.Services;

public interface ITemporalOutboxService : ITemporalOutboxExecutor, ITemporalSignalOutboxExecutor
{
    void StartWorkflowLocationDailyAnalytics(GenerateLocationDailyAnalyticsInput args, IUnitOfWork unitOfWork);

    void StartComputeOrganizationLocationsAndProductsRelationships(
        ComputeOrganizationLocationsAndProductsRelationshipsInput args,
        IUnitOfWork unitOfWork);

    void StartWorkflowNewLocationJoined(NewLocationJoinedInput args, IUnitOfWork unitOfWork);
}

public class TemporalOutboxService(
    ITemporalClient temporalClient,
    TemporalConfiguration temporalConfiguration,
    IWorkflowIdService workflowIdService,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor) : ITemporalOutboxService
{
    private static readonly string s_generateLocationDailyAnalytics = typeof(GenerateLocationDailyAnalytics).ToWorkflowType();

    private static readonly string s_computeOrganizationLocationsAndProductsRelationships =
        typeof(ComputeOrganizationLocationsAndProductsRelationships).ToWorkflowType();

    private static readonly string s_newLocationJoined = typeof(NewLocationJoined).ToWorkflowType();

    public void StartWorkflowLocationDailyAnalytics(GenerateLocationDailyAnalyticsInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<GenerateLocationDailyAnalytics, GenerateLocationDailyAnalyticsInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.GenerateLocationDailyAnalytics(args.LocationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
            },
            unitOfWork);

    public void StartComputeOrganizationLocationsAndProductsRelationships(
        ComputeOrganizationLocationsAndProductsRelationshipsInput args,
        IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor
            .Execute<ComputeOrganizationLocationsAndProductsRelationships, ComputeOrganizationLocationsAndProductsRelationshipsInput>(
                args,
                new WorkflowOptions
                {
                    Id = workflowIdService.ComputeOrganizationLocationsAndProductsRelationships(args.OrganizationId),
                    TaskQueue = temporalConfiguration.Worker.TaskQueue,
                    RetryPolicy = null,
                    IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                    IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting
                },
                unitOfWork);

    public void StartWorkflowNewLocationJoined(NewLocationJoinedInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<NewLocationJoined, NewLocationJoinedInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.NewLocationJoined(args.LocationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_generateLocationDailyAnalytics)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<GenerateLocationDailyAnalyticsInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (GenerateLocationDailyAnalytics workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_computeOrganizationLocationsAndProductsRelationships)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<ComputeOrganizationLocationsAndProductsRelationshipsInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (ComputeOrganizationLocationsAndProductsRelationships workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_newLocationJoined)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<NewLocationJoinedInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((NewLocationJoined workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }

    public Task SignalAsync(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
