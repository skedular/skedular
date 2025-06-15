using Api.Shared.Services;
using Core.Api.Mappers;
using Core.Shared.Models;
using Core.Shared.Repositories;
using Enterprise.Shared.Context;

namespace Core.Api.Services;

public interface ICustomerService
{
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
}

public class CustomerService(IRepositoryFactory repositoryFactory, IMapper mapper, IContext context) : ICustomerService
{
    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken) ??
                       throw new CustomerNotFound();

        return (mapper.MapTo(customer)!, customer);
    }
}
