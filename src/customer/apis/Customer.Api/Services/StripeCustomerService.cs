using Api.Shared.Services;
using Customer.Shared.Database.Entities;
using Customer.Shared.Mappers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Random;
using Stripe;

namespace Customer.Api.Services;

public interface IStripeCustomerService
{
    Task<StripeCustomer> AddAsync(string customerId, CancellationToken cancellationToken);
}

public class StripeCustomerService(
    IRepositoryFactory repositoryFactory,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    ICreatable<Stripe.Customer, CustomerCreateOptions> customerCreateService) : IStripeCustomerService
{
    public async Task<StripeCustomer> AddAsync(string customerId, CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken) ?? throw new CustomerNotFound();
        if (customer.StripeCustomer is not null)
        {
            return customer.StripeCustomer;
        }

        var stripeCustomer = await customerCreateService.CreateAsync(
            entityMapper.MapToStripeCustomerCreateOption(customer),
            new RequestOptions
            {
                IdempotencyKey = customer.Id,
                StripeAccount = null,
            },
            cancellationToken);
        customer.StripeCustomer = repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
        {
            Id = randomHelper.Generate(),
            StripeCustomerId = stripeCustomer.Id,
            Customer = customer,
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return customer.StripeCustomer;
    }
}
