using Api.Shared.Services.Cache;
using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface ICustomerBillingDetailsRepository : IRepository<CustomerBillingDetails>
{
    Task<CustomerBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask AddAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken);
    ValueTask UpdateAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken);
}

public class CustomerBillingDetailsRepository(
    CustomerDbContext dbContext,
    TimeProvider timeProvider,
    IGenericCustomerCacheService genericCustomerCacheService)
    : RepositoryBase<CustomerDbContext, CustomerBillingDetails>(dbContext, timeProvider), ICustomerBillingDetailsRepository
{
    public async Task<CustomerBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.CustomerBillingDetails
            .Include(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async ValueTask AddAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        customerBillingDetails.CreatedAt = now;
        DbContext.CustomerBillingDetails.Add(customerBillingDetails);

        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(
            customerBillingDetails.Customer.Identities.Select(identity => identity.Id),
            cancellationToken);
    }

    public async ValueTask UpdateAsync(CustomerBillingDetails customerBillingDetails, CancellationToken cancellationToken)
    {
        var now = TimeProvider.GetUtcNow();
        customerBillingDetails.ModifiedAt = now;
        DbContext.CustomerBillingDetails.Update(customerBillingDetails);
        await genericCustomerCacheService.InvalidateByVerifiableTokensAsync(
            customerBillingDetails.Customer.Identities.Select(identity => identity.Id),
            cancellationToken);
    }
}
