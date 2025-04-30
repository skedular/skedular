using Enterprise.Shared.Random;
using Payment.Shared.Database.Entities;
using Payment.Shared.Mappers;
using Payment.Shared.Repositories;
using Stripe;
using Customer = Payment.Shared.Models.Customer;
using Organization = Payment.Shared.Models.Organization;

namespace Payment.Shared.Services;

public interface IStripeCustomerService
{
    Task<StripeCustomer> UpsertCustomerAsync(
        Organization organization,
        StripeCustomer? existingStripeCustomer,
        StripeConnectAccount? stripeConnectAccount,
        string? idempotencyKey,
        CancellationToken cancellationToken);

    Task<StripeCustomer> UpsertCustomerAsync(
        Customer customer,
        StripeCustomer? existingStripeCustomer,
        StripeConnectAccount? stripeConnectAccount,
        string? idempotencyKey,
        CancellationToken cancellationToken);
}

public class StripeCustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IRandomHelper randomHelper,
    ICreatable<Stripe.Customer, CustomerCreateOptions> customerCreateService,
    IUpdatable<Stripe.Customer, CustomerUpdateOptions> customerUpdateService) : IStripeCustomerService
{
    public async Task<StripeCustomer> UpsertCustomerAsync(
        Organization organization,
        StripeCustomer? existingStripeCustomer,
        StripeConnectAccount? stripeConnectAccount,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var requestOptions = existingStripeCustomer is null
            ? new RequestOptions
            {
                IdempotencyKey = stripeConnectAccount is null ? organization.Id : $"{organization.Id}-{stripeConnectAccount.StripeAccountId}",
                StripeAccount = stripeConnectAccount?.StripeAccountId
            }
            : new RequestOptions { IdempotencyKey = idempotencyKey, StripeAccount = stripeConnectAccount?.StripeAccountId };

        if (existingStripeCustomer is null)
        {
            var stripeCustomer = await customerCreateService.CreateAsync(
                mapper.MapTo(organization),
                requestOptions,
                cancellationToken);

            return repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
            {
                Id = randomHelper.Generate(), StripeCustomerId = stripeCustomer.Id, StripeConnectAccount = stripeConnectAccount
            });
        }

        _ = await customerUpdateService.UpdateAsync(
            existingStripeCustomer.StripeCustomerId,
            mapper.MergeTo(organization),
            requestOptions,
            cancellationToken);

        return existingStripeCustomer;
    }

    public async Task<StripeCustomer> UpsertCustomerAsync(
        Customer customer,
        StripeCustomer? existingStripeCustomer,
        StripeConnectAccount? stripeConnectAccount,
        string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        var requestOptions = existingStripeCustomer is null
            ? new RequestOptions
            {
                IdempotencyKey = stripeConnectAccount is null ? customer.Id : $"{customer.Id}-{stripeConnectAccount.StripeAccountId}",
                StripeAccount = stripeConnectAccount?.StripeAccountId
            }
            : new RequestOptions { IdempotencyKey = idempotencyKey, StripeAccount = stripeConnectAccount?.StripeAccountId };

        if (existingStripeCustomer is null)
        {
            var stripeCustomer = await customerCreateService.CreateAsync(
                mapper.MapTo(customer),
                requestOptions,
                cancellationToken);

            return repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
            {
                Id = randomHelper.Generate(), StripeCustomerId = stripeCustomer.Id, StripeConnectAccount = stripeConnectAccount
            });
        }

        _ = await customerUpdateService.UpdateAsync(
            existingStripeCustomer.StripeCustomerId,
            mapper.MergeTo(customer),
            requestOptions,
            cancellationToken);

        return existingStripeCustomer;
    }
}
