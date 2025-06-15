using Api.Shared.Services;
using Enterprise.Shared.Context;
using Payment.Api.Mappers;
using Payment.Shared.Models;
using Payment.Shared.Repositories;

namespace Payment.Api.Services;

public interface ICustomerService
{
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
}

public class CustomerService(IRepositoryFactory repositoryFactory, IMapper mapper, IContext context)
    : ICustomerService
{
    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken) ??
                       throw new CustomerNotFound();

        return (mapper.MapTo(customer), customer);
    }
}
