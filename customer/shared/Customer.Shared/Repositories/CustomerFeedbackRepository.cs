using Customer.Shared.Database;
using Enterprise.Shared.Database;
using CustomerFeedback = Customer.Shared.Database.Entities.CustomerFeedback;

namespace Customer.Shared.Repositories;

public interface ICustomerFeedbackRepository : IRepository<Database.Entities.Customer>
{
    CustomerFeedback Add(CustomerFeedback customerFeedback);
}

public class CustomerFeedbackRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Database.Entities.Customer>(dbContext, timeProvider),
        ICustomerFeedbackRepository
{
    public CustomerFeedback Add(CustomerFeedback customerFeedback)
    {
        var now = TimeProvider.GetUtcNow();
        customerFeedback.CreatedAt = now;
        return DbContext.CustomerFeedback.Add(customerFeedback).Entity;
    }
}
