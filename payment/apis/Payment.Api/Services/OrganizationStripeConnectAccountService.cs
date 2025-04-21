using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Payment.Api.Mappers;
using Payment.Api.Services.Authorization;
using Payment.Shared.Repositories;
using Stripe;

namespace Payment.Api.Services;

public interface IOrganizationStripeConnectAccountService
{
    Task AddAsync(string id, string name, CancellationToken cancellationToken);
}

public class OrganizationStripeConnectAccountService(
    IDbTransactionBuilder transactionBuilder,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<Account, AccountCreateOptions> stripeAccountCreateService,
    IMapper mapper) : IOrganizationStripeConnectAccountService
{
    public async Task AddAsync(string id, string name, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

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

        if (string.IsNullOrWhiteSpace(organization.ContactEmail))
        {
            throw new OrganizationContactEmailNotSet();
        }

        if (string.IsNullOrWhiteSpace(organization.ContactPhone))
        {
            throw new OrganizationContactPhoneNotSet();
        }

        if (organization.PhysicalAddress is null)
        {
            throw new OrganizationPhysicalAddressNotSet();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var account = await stripeAccountCreateService.CreateAsync(mapper.MapTo(organization), new RequestOptions(), cancellationToken);

        repositoryFactory.OrganizationStripeConnectAccountRepository.Add(mapper.MapTo(account, name, organization));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
