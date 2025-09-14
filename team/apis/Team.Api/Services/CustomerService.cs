using Api.Shared.Services;
using Enterprise.Shared.Context;
using Team.Api.Mappers;
using Team.Shared.Models;
using Team.Shared.Repositories;

namespace Team.Api.Services;

public interface ICustomerService
{
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
}

public class CustomerService(IRepositoryFactory repositoryFactory, IMapper mapper, IContext context) : ICustomerService
{
    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ??
                       throw new CustomerNotFound();

        return (mapper.MapTo(customer)!, customer);
    }
}
