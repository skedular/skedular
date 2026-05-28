using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface ICustomerBillingDetailsRepository : IRepository<CustomerBillingDetails>
{
    Task<CustomerBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<CustomerBillingDetails?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);
    void Add(CustomerBillingDetails customerBillingDetails);
    void Update(CustomerBillingDetails customerBillingDetails);
}

public class CustomerBillingDetailsRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, CustomerBillingDetails>(dbContext, timeProvider), ICustomerBillingDetailsRepository
{
    public async Task<CustomerBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.CustomerBillingDetails
            .AsSingleQuery()
            .Include(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<CustomerBillingDetails?> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken) =>
        await DbContext.CustomerBillingDetails
            .AsSingleQuery()
            .Include(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .FirstOrDefaultAsync(query => query.Customer.Id == customerId, cancellationToken);

    public void Add(CustomerBillingDetails customerBillingDetails)
    {
        var now = TimeProvider.GetUtcNow();
        customerBillingDetails.CreatedAt = now;
        DbContext.CustomerBillingDetails.Add(customerBillingDetails);
    }

    public void Update(CustomerBillingDetails customerBillingDetails)
    {
        var now = TimeProvider.GetUtcNow();
        customerBillingDetails.ModifiedAt = now;
        DbContext.CustomerBillingDetails.Update(customerBillingDetails);
    }
}
