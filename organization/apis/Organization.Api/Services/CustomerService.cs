using Api.Shared.Services;
using Enterprise.Shared.Context;
using Organization.Api.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface ICustomerService
{
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
    Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(CancellationToken cancellationToken);
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

    public async Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        if (string.IsNullOrWhiteSpace(verifiableToken))
        {
            return (null, null);
        }

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken);
        return customer is null ? (null, null) : (mapper.MapTo(customer)!, customer);
    }
}
