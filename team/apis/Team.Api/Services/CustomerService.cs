using Api.Shared.Services;
using Enterprise.Shared.Context;
using Team.Shared.Database.Entities;
using Team.Shared.Repositories;

namespace Team.Api.Services;

public interface ICustomerService
{
    Task<Customer> GetAsync(CancellationToken cancellationToken);
}

public class CustomerService(IRepositoryFactory repositoryFactory, IContext context, ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<Customer> GetAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        if (string.IsNullOrWhiteSpace(verifiableToken))
        {
            logger.LogWarning("Customer lookup failed because verifiable token is missing");
            ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);
        }

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken);
        if (customer is null)
        {
            logger.LogInformation("Customer lookup returned no result for provided verifiable token context");
            throw new CustomerNotFound();
        }

        return customer;
    }
}
