using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;
using Stripe;
using Customer = Stripe.Customer;

namespace Organization.Api.Services;

public interface IStripeCustomerService
{
    Task<StripeCustomer> AddAsync(string organizationId, CancellationToken cancellationToken);
}

public class StripeCustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IRandomHelper randomHelper,
    ICreatable<Customer, CustomerCreateOptions> customerCreateService) : IStripeCustomerService
{
    public async Task<StripeCustomer> AddAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (organization.StripeCustomer is not null)
        {
            return organization.StripeCustomer;
        }

        var customer = await customerCreateService.CreateAsync(
            mapper.MapToStripeCustomerCreateOption(organization),
            new RequestOptions { IdempotencyKey = organization.Id, StripeAccount = null },
            cancellationToken);
        organization.StripeCustomer = repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
        {
            Id = randomHelper.Generate(), StripeCustomerId = customer.Id, Organization = organization
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return organization.StripeCustomer;
    }
}
