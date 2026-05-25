using Api.Shared.Services;
using Customer.Shared.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Repositories;
using Customer.Shared.Services;
using Customer.Shared.Workflows;
using Enterprise.Shared.Database;
using Stripe;

namespace Customer.Api.Services;

public interface IPaymentService
{
    Task<string> HandleStripePaymentMethodEventAsync(
        string clientSecret,
        string redirectStatus,
        string? redirectTo,
        CancellationToken cancellationToken);

    Task<string> AddPaymentMethodIntentAsync(CancellationToken cancellationToken);
    Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken);
    Task<IReadOnlyList<StripePaymentMethod>> GetPaymentMethodsAsync(string requestedCustomerId, CancellationToken cancellationToken);
    Task<bool> HasAttachedPaymentMethodAsync(string requestedCustomerId, CancellationToken cancellationToken);
}

public class PaymentService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<SetupIntent, SetupIntentCreateOptions> setupIntentCreateService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
    PaymentMethodService paymentMethodService,
    IStripeCustomerService stripeCustomerService,
    ITemporalService temporalService,
    IEntityMapper entityMapper) : IPaymentService
{
    public async Task<string> HandleStripePaymentMethodEventAsync(
        string clientSecret,
        string redirectStatus,
        string? redirectTo,
        CancellationToken cancellationToken) =>
        await temporalService.SignalAddCustomerStripePaymentMethodAndGetResultAsync(
            clientSecret,
            new StripePaymentMethodEventState(redirectStatus, redirectTo),
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
        _ = await customerService.GetCustomerAsync(cancellationToken);
        var stripePaymentMethod = await repositoryFactory.StripePaymentMethodRepository.GetByIdAsync(paymentMethodId, cancellationToken) ??
                                  throw new OrganizationPaymentMethodNotFound();
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.StripePaymentMethodRepository.Remove(stripePaymentMethod);

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

    public async Task<IReadOnlyList<StripePaymentMethod>> GetPaymentMethodsAsync(string requestedCustomerId, CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        if (customerEntity.Id != requestedCustomerId)
        {
            return [];
        }

        var stripePaymentMethods = await repositoryFactory.StripePaymentMethodRepository.GetByCustomerIdAsync(customerEntity.Id, cancellationToken);

        return entityMapper.MapTo(stripePaymentMethods).ToList();
    }

    public async Task<bool> HasAttachedPaymentMethodAsync(string requestedCustomerId, CancellationToken cancellationToken) =>
        (await GetPaymentMethodsAsync(requestedCustomerId, cancellationToken)).Any();
}
