using Customer.Shared.Workflows;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Customer.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowAddCustomerStripePaymentMethodAsync(AddCustomerStripePaymentMethodInput args, CancellationToken cancellationToken);

    Task<string> SignalAddCustomerStripePaymentMethodAndGetResultAsync(
        string clientSecret,
        StripePaymentMethodEventState args,
        CancellationToken cancellationToken);
}

public class TemporalService(
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient,
    ITemporalHelperService temporalHelperService) : ITemporalService
{
    public async Task StartWorkflowAddCustomerStripePaymentMethodAsync(
        AddCustomerStripePaymentMethodInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((AddCustomerStripePaymentMethod workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId(args.ClientSecret),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task<string> SignalAddCustomerStripePaymentMethodAndGetResultAsync(
        string clientSecret,
        StripePaymentMethodEventState args,
        CancellationToken cancellationToken)
    {
        var handle = temporalClient.GetWorkflowHandle<AddCustomerStripePaymentMethod>(temporalHelperService.ToId(clientSecret));

        await handle.SignalAsync(
            workflow => workflow.StripePaymentMethodEventReceivedAsync(args),
            new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } }
        );

        return await handle.GetResultAsync<string>(rpcOptions: new RpcOptions { CancellationToken = cancellationToken });
    }
}
