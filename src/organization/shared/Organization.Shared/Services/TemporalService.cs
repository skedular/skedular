using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowGenerateOrganizationDailyAnalyticsAsync(GenerateOrganizationDailyAnalyticsInput args, CancellationToken cancellationToken);

    Task StartOrSignalWorkflowRecomputeOrganizationBookingDerivedStateAsync(
        RecomputeOrganizationBookingDerivedStateInput args,
        CancellationToken cancellationToken);

    Task StartWorkflowReSyncAzureTenantAsync(ReSyncAzureTenantInput args, CancellationToken cancellationToken);
    Task StartWorkflowAddOrganizationStripePaymentMethodAsync(AddOrganizationStripePaymentMethodInput args, CancellationToken cancellationToken);

    Task<string> SignalAddOrganizationStripePaymentMethodAndGetResultAsync(
        string clientSecret,
        StripePaymentMethodEventState args,
        CancellationToken cancellationToken);
}

public class TemporalService(
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient,
    IWorkflowIdService workflowIdService) : ITemporalService
{
    public async Task StartWorkflowGenerateOrganizationDailyAnalyticsAsync(
        GenerateOrganizationDailyAnalyticsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateOrganizationDailyAnalytics workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = workflowIdService.GenerateOrganizationDailyAnalytics(args.OrganizationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions
                {
                    CancellationToken = cancellationToken,
                },
            });

    public async Task StartOrSignalWorkflowRecomputeOrganizationBookingDerivedStateAsync(
        RecomputeOrganizationBookingDerivedStateInput args,
        CancellationToken cancellationToken)
    {
        var workflowOptions = new WorkflowOptions
        {
            Id = workflowIdService.RecomputeOrganizationBookingDerivedState(args.OrganizationId),
            TaskQueue = temporalConfiguration.Worker.TaskQueue,
            RetryPolicy = null,
            IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
            Rpc = new RpcOptions
            {
                CancellationToken = cancellationToken,
            },
        };

        workflowOptions.SignalWithStart((RecomputeOrganizationBookingDerivedState workflow) => workflow.BookingChangedAsync());

        await temporalClient.StartWorkflowAsync(
            (RecomputeOrganizationBookingDerivedState workflow) => workflow.ExecuteAsync(args),
            workflowOptions);
    }

    public async Task StartWorkflowReSyncAzureTenantAsync(ReSyncAzureTenantInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ReSyncAzureTenant workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = workflowIdService.ReSyncAzureTenant(args.TenantId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions
                {
                    CancellationToken = cancellationToken,
                },
            });

    public async Task StartWorkflowAddOrganizationStripePaymentMethodAsync(
        AddOrganizationStripePaymentMethodInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((AddOrganizationStripePaymentMethod workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = workflowIdService.AddOrganizationStripePaymentMethod(args.ClientSecret),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
                Rpc = new RpcOptions
                {
                    CancellationToken = cancellationToken,
                },
            });

    public async Task<string> SignalAddOrganizationStripePaymentMethodAndGetResultAsync(
        string clientSecret,
        StripePaymentMethodEventState args,
        CancellationToken cancellationToken)
    {
        var handle = temporalClient.GetWorkflowHandle<AddOrganizationStripePaymentMethod>(
            workflowIdService.AddOrganizationStripePaymentMethod(clientSecret));

        await handle.SignalAsync(
            workflow => workflow.StripePaymentMethodEventReceivedAsync(args),
            new WorkflowSignalOptions
            {
                Rpc = new RpcOptions
                {
                    CancellationToken = cancellationToken,
                },
            }
        );

        return await handle.GetResultAsync<string>(rpcOptions: new RpcOptions
        {
            CancellationToken = cancellationToken,
        });
    }
}
