using Customer.Api.Mappers;
using Customer.Shared.Database.Entities;
using Customer.Shared.Repositories;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Stripe;

namespace Customer.Api.Services;

public interface IStripeCustomerService
{
    Task<StripeCustomer> AddAsync(string organizationId, CancellationToken cancellationToken);
}

public class StripeCustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IRandomHelper randomHelper,
    ICreatable<Stripe.Customer, CustomerCreateOptions> customerCreateService) : IStripeCustomerService
{
    public async Task<StripeCustomer> AddAsync(string organizationId, CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(organizationId, cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        if (customer.StripeCustomer is not null)
        {
            return customer.StripeCustomer;
        }

        var stripeCustomer = await customerCreateService.CreateAsync(
            mapper.MapToStripeCustomerCreateOption(customer),
            new RequestOptions { IdempotencyKey = customer.Id, StripeAccount = null },
            cancellationToken);
        customer.StripeCustomer = repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
        {
            Id = randomHelper.Generate(), StripeCustomerId = stripeCustomer.Id, Customer = customer
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return customer.StripeCustomer;
    }
}
