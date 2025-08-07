using Customer.Shared.Database;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using CustomerFeedback = Customer.Shared.Database.Entities.CustomerFeedback;

namespace Customer.Shared.Repositories;

public interface ICustomerFeedbackRepository : IRepository<Database.Entities.Customer>
{
    Task<CustomerFeedback?> GetByIdAsync(string id, CancellationToken cancellationToken);
    CustomerFeedback Add(CustomerFeedback customerFeedback);
}

public class CustomerFeedbackRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Database.Entities.Customer>(dbContext, timeProvider), ICustomerFeedbackRepository
{
    public async Task<CustomerFeedback?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.CustomerFeedback
            .Include(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public CustomerFeedback Add(CustomerFeedback customerFeedback)
    {
        var now = TimeProvider.GetUtcNow();
        customerFeedback.CreatedAt = now;
        return DbContext.CustomerFeedback.Add(customerFeedback).Entity;
    }
}
