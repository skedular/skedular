using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface ICustomerBillingDetailsRepository : IRepository<CustomerBillingDetails>
{
    Task<CustomerBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken);
    CustomerBillingDetails Add(CustomerBillingDetails customerBillingDetails);
    CustomerBillingDetails Update(CustomerBillingDetails customerBillingDetails);
}

public class CustomerBillingDetailsRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, CustomerBillingDetails>(dbContext, timeProvider), ICustomerBillingDetailsRepository
{
    public async Task<CustomerBillingDetails?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.CustomerBillingDetails
            .Include(query => query.Customer)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public CustomerBillingDetails Add(CustomerBillingDetails customerBillingDetails)
    {
        var now = TimeProvider.GetUtcNow();
        customerBillingDetails.CreatedAt = now;
        return DbContext.CustomerBillingDetails.Add(customerBillingDetails).Entity;
    }

    public CustomerBillingDetails Update(CustomerBillingDetails customerBillingDetails)
    {
        var now = TimeProvider.GetUtcNow();
        customerBillingDetails.ModifiedAt = now;
        return DbContext.CustomerBillingDetails.Update(customerBillingDetails).Entity;
    }
}
