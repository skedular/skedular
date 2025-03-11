using Enterprise.Shared.Context;
using Enterprise.Shared.Exceptions;
using Payment.Api.Mappers;
using Payment.Shared.Models;
using Payment.Shared.Repositories;

namespace Payment.Api.Services;

public interface ICustomerService
{
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
    Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(CancellationToken cancellationToken);
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(string id, CancellationToken cancellationToken);
}

public class CustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IContext context) : ICustomerService
{
    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        return (mapper.MapTo(customer), customer);
    }

    public async Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.GetVerifiableToken()))
        {
            return (null, null);
        }

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
        return customer is null ? (null, null) : (mapper.MapTo(customer), customer);
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        return (mapper.MapTo(customer), customer);
    }
}
