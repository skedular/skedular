using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Random;
using Stripe;
using Customer = Booking.Shared.Database.Entities.Customer;

namespace Booking.Shared.Services;

public interface IStripeCustomerService
{
    Task<StripeCustomer> AddCustomerAsync(Organization organization, string stripeAccountId, CancellationToken cancellationToken);
    Task<StripeCustomer> AddCustomerAsync(Customer customerEntity, string stripeAccountId, CancellationToken cancellationToken);
}

public class StripeCustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IRandomHelper randomHelper,
    ICreatable<Stripe.Customer, CustomerCreateOptions> customerCreateService) : IStripeCustomerService
{
    public async Task<StripeCustomer> AddCustomerAsync(Organization organization, string stripeAccountId, CancellationToken cancellationToken)
    {
        var existingStripeCustomer =
            await repositoryFactory.StripeCustomerRepository.GetByOrganizationIdAsync(stripeAccountId, organization.Id, cancellationToken);

        if (existingStripeCustomer is not null)
        {
            return existingStripeCustomer;
        }

        var stripeCustomer = await customerCreateService.CreateAsync(
            mapper.MapToCustomerCreateOption(organization),
            new RequestOptions { IdempotencyKey = $"{organization.Id}-{stripeAccountId}", StripeAccount = stripeAccountId },
            cancellationToken);

        return repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
        {
            Id = randomHelper.Generate(), StripeCustomerId = stripeCustomer.Id, StripeAccountId = stripeAccountId, Organization = organization
        });
    }

    public async Task<StripeCustomer> AddCustomerAsync(Customer customer, string stripeAccountId, CancellationToken cancellationToken)
    {
        var existingStripeCustomer =
            await repositoryFactory.StripeCustomerRepository.GetByCustomerIdAsync(stripeAccountId, customer.Id, cancellationToken);

        if (existingStripeCustomer is not null)
        {
            return existingStripeCustomer;
        }

        var stripeCustomer = await customerCreateService.CreateAsync(
            mapper.MapToCustomerCreateOption(customer),
            new RequestOptions { IdempotencyKey = $"{customer.Id}-{stripeAccountId}", StripeAccount = stripeAccountId },
            cancellationToken);

        return repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
        {
            Id = randomHelper.Generate(), StripeCustomerId = stripeCustomer.Id, StripeAccountId = stripeAccountId, Customer = customer
        });
    }
}
