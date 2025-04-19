using Billing.Api.Mappers;
using Billing.Shared.Models;
using Billing.Shared.Publishers;
using Billing.Shared.Repositories;
using Enterprise.Shared.Database;

namespace Billing.Api.Services;

public interface ICustomerBillingService
{
    Task<Customer> GetMyBillingContact(CancellationToken cancellationToken);

    Task<Customer> UpdateMyBillingInfoAsync(
        string? companyName,
        string? email,
        string? addressLine1,
        string? addressLine2,
        string? suburb,
        string? city,
        string? province,
        string? zipcode,
        string? country,
        CancellationToken cancellationToken);
}

public class CustomerBillingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IMapper mapper,
    IBillingOutboxPublisher billingOutboxPublisher)
    : ICustomerBillingService
{
    public async Task<Customer> GetMyBillingContact(CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);

        return customer;
    }

    public async Task<Customer> UpdateMyBillingInfoAsync(
        string? companyName,
        string? email,
        string? addressLine1,
        string? addressLine2,
        string? suburb,
        string? city,
        string? province,
        string? zipcode,
        string? country,
        CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await cachedCustomerService.GetAsync(cancellationToken);
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        customerEntity.BillingContactCompanyName = companyName;
        customerEntity.BillingContactEmail = email;
        customerEntity.BillingContactAddressLine1 = addressLine1;
        customerEntity.BillingContactAddressLine2 = addressLine2;
        customerEntity.BillingContactSuburb = suburb;
        customerEntity.BillingContactCity = city;
        customerEntity.BillingContactProvince = province;
        customerEntity.BillingContactZipcode = zipcode;
        customerEntity.BillingContactCountry = country;

        var customer = mapper.MapTo(repositoryFactory.CustomerRepository.Update(customerEntity));

        billingOutboxPublisher.PublishCustomersBillingInfo([customer], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return customer;
    }
}
