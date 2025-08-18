using Api.Shared.Services;
using Customer.Shared.Repositories;
using Customer.Shared.Services;
using Customer.Shared.Workflows.AddPayment;
using Enterprise.Shared.Database;
using Stripe;

namespace Customer.Api.Services;

public interface IPaymentService
{
    Task<string> HandleStripePaymentMethodEventAsync(string clientSecret, string redirectStatus, CancellationToken cancellationToken);
    Task<string> AddPaymentMethodIntentAsync(CancellationToken cancellationToken);
    Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken);
}

public class PaymentService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<SetupIntent, SetupIntentCreateOptions> setupIntentCreateService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
    PaymentMethodService paymentMethodService,
    IStripeCustomerService stripeCustomerService,
    ITemporalService temporalService) : IPaymentService
{
    public async Task<string> HandleStripePaymentMethodEventAsync(string clientSecret, string redirectStatus, CancellationToken cancellationToken) =>
        await temporalService.SignalAddCustomerStripePaymentMethodAndGetResultAsync(
            clientSecret,
            new StripePaymentMethodEventState(redirectStatus),
            cancellationToken);

    public async Task<string> AddPaymentMethodIntentAsync(CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);

        customerEntity.StripeCustomer ??= await stripeCustomerService.AddAsync(customerEntity.Id, cancellationToken);

        var setupIntent = await setupIntentCreateService.CreateAsync(
            new SetupIntentCreateOptions { Customer = customerEntity.StripeCustomer.StripeCustomerId, PaymentMethodTypes = ["card"] },
            new RequestOptions(),
            cancellationToken);

        await temporalService.StartWorkflowAddCustomerStripePaymentMethodAsync(
            new AddCustomerStripePaymentMethodInput(customerEntity.Id, setupIntent.ClientSecret, setupIntent.Id),
            cancellationToken);

        return setupIntent.ClientSecret;
    }

    public async Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var stripePaymentMethod = await repositoryFactory.StripePaymentMethodRepository.GetByIdAsync(paymentMethodId, cancellationToken) ??
                                  throw new OrganizationPaymentMethodNotFound();
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        await repositoryFactory.StripePaymentMethodRepository.RemoveAsync(stripePaymentMethod, cancellationToken);
        await repositoryFactory.CustomerRepository.UpdateAsync(customerEntity, cancellationToken);

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
    }
}
