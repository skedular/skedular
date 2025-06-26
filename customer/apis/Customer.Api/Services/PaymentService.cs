using Api.Shared.Services;
using Customer.Shared.Repositories;
using Customer.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Temporal.Configurations;
using Stripe;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Customer.Api.Services;

public interface IPaymentService
{
    Task<string> HandleStripePaymentMethodEventAsync(
        string setupIntentId,
        string clientSecret,
        string redirectStatus,
        CancellationToken cancellationToken);

    Task<string> AddPaymentMethodIntentAsync(CancellationToken cancellationToken);
    Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken);
}

public class PaymentService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICachedCustomerService cachedCustomerService,
    ICreatable<SetupIntent, SetupIntentCreateOptions> setupIntentCreateService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
    PaymentMethodService paymentMethodService,
    IStripeCustomerService stripeCustomerService,
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient) : IPaymentService
{
    public async Task<string> HandleStripePaymentMethodEventAsync(
        string setupIntentId,
        string clientSecret,
        string redirectStatus,
        CancellationToken cancellationToken)
    {
        var handle = temporalClient.GetWorkflowHandle<AddCustomerStripePaymentMethod>(clientSecret);

        ArgumentNullException.ThrowIfNull(handle);

        await handle.SignalAsync(
            workflow => workflow.StripePaymentMethodEventReceivedAsync(new StripePaymentMethodEventState(redirectStatus)),
            new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } }
        );

        var redirectUrl = await handle.GetResultAsync<string>(rpcOptions: new RpcOptions { CancellationToken = cancellationToken });

        var stripePaymentMethod = await repositoryFactory.StripePaymentMethodRepository.GetBySetupIntentIdAsync(setupIntentId, cancellationToken);
        if (stripePaymentMethod is not null)
        {
            cachedCustomerService.CleanCache(stripePaymentMethod.Customer);
        }

        return redirectUrl;
    }

    public async Task<string> AddPaymentMethodIntentAsync(CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);

        customerEntity.StripeCustomer ??= await stripeCustomerService.AddAsync(customerEntity.Id, cancellationToken);

        var setupIntent = await setupIntentCreateService.CreateAsync(
            new SetupIntentCreateOptions { Customer = customerEntity.StripeCustomer.StripeCustomerId, PaymentMethodTypes = ["card"] },
            new RequestOptions(),
            cancellationToken);

        _ = await temporalClient.StartWorkflowAsync(
            (AddCustomerStripePaymentMethod workflow) =>
                workflow.ExecuteAsync(new AddCustomerStripePaymentMethodInput(customerEntity.Id, setupIntent.ClientSecret, setupIntent.Id)),
            new WorkflowOptions
            {
                Id = setupIntent.ClientSecret,
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

        return setupIntent.ClientSecret;
    }

    public async Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var stripePaymentMethod = await repositoryFactory.StripePaymentMethodRepository.GetByIdAsync(paymentMethodId, cancellationToken) ??
                                  throw new OrganizationPaymentMethodNotFound();
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.StripePaymentMethodRepository.Remove(stripePaymentMethod);
        repositoryFactory.CustomerRepository.Update(customerEntity);

        var paymentMethod = await paymentMethodRetrievableService.GetAsync(stripePaymentMethod.PaymentMethodId, cancellationToken: cancellationToken);
        if (paymentMethod is not null)
        {
            await paymentMethodService.DetachAsync(
                stripePaymentMethod.PaymentMethodId,
                new PaymentMethodDetachOptions(),
                new RequestOptions { IdempotencyKey = $"DetachPaymentMethod-{stripePaymentMethod.Id}" },
                cancellationToken);
        }

        _ = await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        cachedCustomerService.CleanCache(customerEntity);
    }
}
