using Api.Shared.Services;
using Enterprise.Shared.Context;
using Team.Shared.Database.Entities;
using Team.Shared.Repositories;

namespace Team.Api.Services;

public interface ICustomerService
{
    Task<Customer> GetAsync(CancellationToken cancellationToken);
}

public class CustomerService(IRepositoryFactory repositoryFactory, IContext context) : ICustomerService
{
    public async Task<Customer> GetAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ??
               throw new CustomerNotFound();
    }
}
