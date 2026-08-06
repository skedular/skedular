using System.Text.Json;
using Customer.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Customer.Shared.Services;

public interface ITemporalOutboxService : ITemporalOutboxExecutor, ITemporalSignalOutboxExecutor
{
    void StartWorkflowSubmitCustomerFeedback(SubmitCustomerFeedbackInput args, IUnitOfWork unitOfWork);
    void StartWorkflowNewCustomerJoined(NewCustomerJoinedInput args, IUnitOfWork unitOfWork);
}

public class TemporalOutboxService(
    ITemporalClient temporalClient,
    TemporalConfiguration temporalConfiguration,
    IWorkflowIdService workflowIdService,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor) : ITemporalOutboxService
{
    private static readonly string s_submitCustomerFeedback = typeof(SubmitCustomerFeedback).ToWorkflowType();
    private static readonly string s_newCustomerJoined = typeof(NewCustomerJoined).ToWorkflowType();

    public void StartWorkflowSubmitCustomerFeedback(SubmitCustomerFeedbackInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<SubmitCustomerFeedback, SubmitCustomerFeedbackInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.SubmitCustomerFeedback(args.CustomerFeedbackId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
            },
            unitOfWork);

    public void StartWorkflowNewCustomerJoined(NewCustomerJoinedInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<NewCustomerJoined, NewCustomerJoinedInput>(
            args,
            new WorkflowOptions
            {
                Id = workflowIdService.NewCustomerJoined(args.CustomerId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
            },
            unitOfWork);

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_submitCustomerFeedback)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<SubmitCustomerFeedbackInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((SubmitCustomerFeedback workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_newCustomerJoined)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<NewCustomerJoinedInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((NewCustomerJoined workflow) => workflow.ExecuteAsync(input), workflowOptions);
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
