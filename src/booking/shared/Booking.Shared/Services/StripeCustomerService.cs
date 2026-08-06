using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Random;
using Stripe;
using Customer = Booking.Shared.Database.Entities.Customer;

namespace Booking.Shared.Services;

/// <summary>
///     Service for managing Stripe customer creation and retrieval.
/// </summary>
public interface IStripeCustomerService
{
    /// <summary>
    ///     Adds or retrieves a Stripe customer for an organization.
    ///     If a customer already exists, returns the existing one.
    /// </summary>
    /// <param name="organization">The organization entity.</param>
    /// <param name="stripeAccountId">The Stripe account ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The Stripe customer entity.</returns>
    Task<StripeCustomer> AddCustomerAsync(Organization organization, string stripeAccountId, CancellationToken cancellationToken);

    /// <summary>
    ///     Adds or retrieves a Stripe customer for a customer entity.
    ///     If a customer already exists, returns the existing one.
    /// </summary>
    /// <param name="customerEntity">The customer entity.</param>
    /// <param name="stripeAccountId">The Stripe account ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The Stripe customer entity.</returns>
    Task<StripeCustomer> AddCustomerAsync(Customer customerEntity, string stripeAccountId, CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the Stripe customer service.
/// </summary>
public class StripeCustomerService(
    IRepositoryFactory repositoryFactory,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    ICreatable<Stripe.Customer, CustomerCreateOptions> customerCreateService) : IStripeCustomerService
{
    /// <summary>
    ///     Adds or retrieves a Stripe customer for an organization.
    ///     If a customer already exists, returns the existing one.
    /// </summary>
    /// <param name="organization">The organization entity.</param>
    /// <param name="stripeAccountId">The Stripe account ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The Stripe customer entity.</returns>
    public async Task<StripeCustomer> AddCustomerAsync(Organization organization, string stripeAccountId, CancellationToken cancellationToken)
    {
        var existingStripeCustomer =
            await repositoryFactory.StripeCustomerRepository.GetByOrganizationIdAsync(stripeAccountId, organization.Id, cancellationToken);

        if (existingStripeCustomer is not null)
        {
            return existingStripeCustomer;
        }

        var stripeCustomer = await customerCreateService.CreateAsync(
            entityMapper.MapToCustomerCreateOption(organization),
            new RequestOptions
            {
                IdempotencyKey = $"{organization.Id}-{stripeAccountId}",
                StripeAccount = stripeAccountId,
            },
            cancellationToken);

        return repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
        {
            Id = randomHelper.Generate(),
            StripeCustomerId = stripeCustomer.Id,
            StripeAccountId = stripeAccountId,
            Organization = organization,
        });
    }

    /// <summary>
    ///     Adds or retrieves a Stripe customer for a customer entity.
    ///     If a customer already exists, returns the existing one.
    /// </summary>
    /// <param name="customer">The customer entity.</param>
    /// <param name="stripeAccountId">The Stripe account ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The Stripe customer entity.</returns>
    public async Task<StripeCustomer> AddCustomerAsync(Customer customer, string stripeAccountId, CancellationToken cancellationToken)
    {
        var existingStripeCustomer =
            await repositoryFactory.StripeCustomerRepository.GetByCustomerIdAsync(stripeAccountId, customer.Id, cancellationToken);

        if (existingStripeCustomer is not null)
        {
            return existingStripeCustomer;
        }

        var stripeCustomer = await customerCreateService.CreateAsync(
            entityMapper.MapToCustomerCreateOption(customer),
            new RequestOptions
            {
                IdempotencyKey = $"{customer.Id}-{stripeAccountId}",
                StripeAccount = stripeAccountId,
            },
            cancellationToken);

        return repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
        {
            Id = randomHelper.Generate(),
            StripeCustomerId = stripeCustomer.Id,
            StripeAccountId = stripeAccountId,
            Customer = customer,
        });
    }
}
