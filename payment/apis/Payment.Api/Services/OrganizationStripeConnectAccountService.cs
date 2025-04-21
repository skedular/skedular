using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Payment.Api.Services.Authorization;
using Payment.Shared.Repositories;
using Stripe;

namespace Payment.Api.Services;

public interface IOrganizationStripeConnectAccountService
{
    Task AddAsync(string id, CancellationToken cancellationToken);
}

public class OrganizationStripeConnectAccountService(
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<Account, AccountCreateOptions> stripeAccountCreateService,
    IRandomHelper randomHelper) : IOrganizationStripeConnectAccountService
{
    public async Task AddAsync(string id, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(id, false, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanManageStripeConnectAccount(organization, customer))
        {
            throw new Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(organization.Name))
        {
            throw new OrganizationNameIsInvalid();
        }

        await stripeAccountCreateService.CreateAsync(
            new AccountCreateOptions { Company = new AccountCompanyOptions { Name = organization.Name } },
            new RequestOptions { IdempotencyKey = randomHelper.Generate() }, cancellationToken);

        throw new NotImplementedException();
    }
}
