using Api.Shared.Services;
using Core.Shared.Mappers;
using Core.Shared.Models;
using Core.Shared.Repositories;
using Enterprise.Shared.Context;

namespace Core.Api.Services;

public interface ICustomerService
{
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
}

public class CustomerService(IRepositoryFactory repositoryFactory, IEntityMapper entityMapper, IContext context) : ICustomerService
{
    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ??
                       throw new CustomerNotFound();

        return (entityMapper.MapTo(customer)!, customer);
    }
}
