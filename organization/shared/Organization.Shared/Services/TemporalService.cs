using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Workflows;
using Organization.Shared.Workflows.AddPayment;
using Organization.Shared.Workflows.GenerateOrganizationDailyAnalytics;
using Organization.Shared.Workflows.ReSyncAzureTenant;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowGenerateOrganizationDailyAnalyticsAsync(GenerateOrganizationDailyAnalyticsInput args, CancellationToken cancellationToken);
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
    ITemporalHelperService temporalHelperService) : ITemporalService
{
    public async Task StartWorkflowGenerateOrganizationDailyAnalyticsAsync(
        GenerateOrganizationDailyAnalyticsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateOrganizationDailyAnalytics workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.GenerateOrganizationDailyAnalyticsPrefix}-{args.OrganizationId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartWorkflowReSyncAzureTenantAsync(ReSyncAzureTenantInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ReSyncAzureTenant workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.ReSyncAzureTenantPrefix}-{args.TenantId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartWorkflowAddOrganizationStripePaymentMethodAsync(
        AddOrganizationStripePaymentMethodInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((AddOrganizationStripePaymentMethod workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(args.ClientSecret),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task<string> SignalAddOrganizationStripePaymentMethodAndGetResultAsync(
        string clientSecret,
        StripePaymentMethodEventState args,
        CancellationToken cancellationToken)
    {
        var handle = temporalClient.GetWorkflowHandle<AddOrganizationStripePaymentMethod>(temporalHelperService.ToId(clientSecret));

        await handle.SignalAsync(
            workflow => workflow.StripePaymentMethodEventReceivedAsync(args),
            new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } }
        );

        return await handle.GetResultAsync<string>(rpcOptions: new RpcOptions { CancellationToken = cancellationToken });
    }
}
