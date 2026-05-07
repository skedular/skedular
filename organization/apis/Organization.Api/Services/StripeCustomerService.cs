using Api.Shared.Services;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;
using Stripe;
using Customer = Stripe.Customer;

namespace Organization.Api.Services;

public interface IStripeCustomerService
{
    Task<OrganizationStripeCustomer> AddAsync(string organizationId, CancellationToken cancellationToken);
}

public class StripeCustomerService(
    IRepositoryFactory repositoryFactory,
    IGraphQlMapper graphQlMapper,
    IRandomHelper randomHelper,
    ICreatable<Customer, CustomerCreateOptions> customerCreateService) : IStripeCustomerService
{
    public async Task<OrganizationStripeCustomer> AddAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (organization.OrganizationStripeCustomer is not null)
        {
            return organization.OrganizationStripeCustomer;
        }

        var stripeCustomer = await customerCreateService.CreateAsync(
            graphQlMapper.MapToStripeCustomerCreateOption(organization),
            new RequestOptions { IdempotencyKey = organization.Id, StripeAccount = null },
            cancellationToken);
        organization.OrganizationStripeCustomer = repositoryFactory.OrganizationStripeCustomerRepository.Add(new OrganizationStripeCustomer
        {
            Id = randomHelper.Generate(), StripeCustomerId = stripeCustomer.Id, Organization = organization
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return organization.OrganizationStripeCustomer;
    }
}
